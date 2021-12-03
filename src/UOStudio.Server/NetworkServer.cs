using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Layers;
using LiteNetLib.Utils;
using MediatR;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Microsoft.Xna.Framework;
using Serilog;
using UOStudio.Common.Network;
using UOStudio.Server.Common;

namespace UOStudio.Server
{
    public class NetworkServer : INetworkServer
    {
        private readonly ILogger _logger;
        private readonly ServerSettings _serverSettings;
        private readonly IMediator _mediator;
        private readonly NetManager _server;

        private readonly Thread _clientThread;
        private readonly CancellationTokenSource _cancellationToken;
        private readonly SynchronizedCollection<NetworkClient> _clients;

        private readonly ObjectPool<NetDataWriter> _writerPool;

        public NetworkServer(
            ILogger logger,
            IOptions<ServerSettings> serverSettings,
            IMediator mediator)
        {
            _logger = logger;
            _serverSettings = serverSettings.Value;
            _mediator = mediator;

            _writerPool = new DefaultObjectPool<NetDataWriter>(new DefaultPooledObjectPolicy<NetDataWriter>());

            var listener = new EventBasedNetListener();
            listener.ConnectionRequestEvent += ListenerOnConnectionRequestEvent;
            listener.PeerConnectedEvent += ListenerOnPeerConnectedEvent;
            listener.PeerDisconnectedEvent += ListenerOnPeerDisconnectedEvent;
            listener.NetworkReceiveEvent += async (peer, reader, deliveryMethod) => await ListenerOnNetworkReceiveEvent(peer, reader, deliveryMethod);

            _server = new NetManager(listener, new XorEncryptLayer("UOStudio"));
            _clients = new SynchronizedCollection<NetworkClient>(16);
            _clientThread = new Thread(ClientThreadProc);
            _cancellationToken = new CancellationTokenSource();
        }

        public void Dispose()
        {
            _cancellationToken?.Dispose();
        }

        public void Start(int port)
        {
            _logger.Debug("NetworkServer: Starting...");
            _server.Start(port);
            _clientThread.Start();
            _logger.Debug("NetworkServer: Starting...Done. Running on port {@Port}", port);
        }

        public void Stop()
        {
            _logger.Debug("NetworkServer: Stopping...");
            _cancellationToken.Cancel();
            _server.Stop();
            _logger.Debug("NetworkServer: Stopping...Done");
        }

        private async Task ListenerOnNetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var client = GetClientFromPeer(peer);
            if (client == null)
            {
                _logger.Error("NetworkServer: Packet received from unlisted client");
                return;
            }

            var packetId = reader.GetInt();
            _logger.Debug("NetworkServer: Packet {@PacketId} from client {@ClientId} ({@ClientEndPoint}:{@ClientPort})", packetId, client.Id, client.Address, peer.EndPoint.Port);
            switch (packetId)
            {
                case PacketIds.C2S.Chat:
                    HandleChat(client, reader);
                    break;
                case PacketIds.C2S.RequestChunk:
                    HandleRequestChunk(client, reader);
                    break;
                case PacketIds.C2S.JoinWorld:
                    HandleJoinWorld(client, reader);
                    break;
                case PacketIds.C2S.LeaveWorld:
                    HandleLeaveWorld(client, reader);
                    break;
            }
        }

        private void ListenerOnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            var client = GetClientFromPeer(peer);
            if (client == null)
            {
                _logger.Debug("NetworkServer: Client {@Id} was not connected", peer.Id);
            }
            else
            {
                _logger.Information("NetworkServer: Client {@Id} disconnected", client.Id);
                _clients.Remove(client);
            }
        }

        private void ListenerOnPeerConnectedEvent(NetPeer peer)
        {
            _logger.Information("NetworkServer: Client {@Id} connected", peer.Id);

            var networkClient = new NetworkClient(peer);
            if (!_clients.Contains(networkClient))
            {
                _clients.Add(networkClient);
            }
        }

        private void ListenerOnConnectionRequestEvent(ConnectionRequest connectionRequest)
        {
            var remoteEndpoint = connectionRequest.RemoteEndPoint.ToString();
            _logger.Debug("NetworkServer: Connection request from {@RemoteEndPoint}", remoteEndpoint);
            if (_server.ConnectedPeersCount < _serverSettings.MaximumConnectedPeersCount)
            {
                connectionRequest.AcceptIfKey("UOStudio");
                _logger.Debug("NetworkServer: Connection request from {@RemoteEndPoint} accepted", remoteEndpoint);
            }
            else
            {
                connectionRequest.Reject();
                _logger.Debug("NetworkServer: Connection request from {@RemoteEndPoint} rejected", remoteEndpoint);
            }
        }

        private void ClientThreadProc(object state)
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                _server.PollEvents();
            }
        }

        private NetworkClient GetClientFromPeer(NetPeer peer)
        {
            return _clients.FirstOrDefault(client => client.Id == peer.Id);
        }

        private void HandleRequestChunk(NetworkClient client, NetDataReader reader)
        {
            var worldId = reader.GetInt();
            var chunkPositionX = reader.GetInt();
            var chunkPositionY = reader.GetInt();

            // ask worldProvider for world by Id
            // get world chunk for chunkPosition
            // serialize chunkdata

            var chunkLength = ChunkData.ChunkSize * ChunkData.ChunkSize;
            var chunkStaticTileData = new ChunkStaticTileData[chunkLength];
            for (var i = 0; i < chunkLength; i++)
            {
                ushort tileId = 100;
                var z = 0;
                var hue = 0;
                chunkStaticTileData[i] = new ChunkStaticTileData(tileId, z, hue);
            }

            var chunkItemTileData = new ChunkItemTileData[chunkLength];
            for (var i = 0; i < chunkLength; i++)
            {
                ushort tileId = 25000;
                var z = 10;
                var hue = 0;
                chunkItemTileData[i] = new ChunkItemTileData(tileId, z, hue);
            }

            var chunkData = new ChunkData(worldId, new Point(chunkPositionX, chunkPositionY), chunkStaticTileData, chunkItemTileData);
            var writer = _writerPool.Get();
            writer.Put(PacketIds.S2C.RequestedChunk);
            writer.Put(chunkData);

            client.Send(writer);
            _writerPool.Return(writer);
            _logger.Debug("NetworkServer: Sent ChunkData for Chunk {@ChunkPositionX}, {@ChunkPositionY} to {@ClientAddress}:{@ClientPort}", chunkPositionX, chunkPositionY, client.Address, client.Port);
        }

        private void HandleJoinWorld(NetworkClient client, NetDataReader reader)
        {
            var worldId = reader.GetInt();
            var writer = _writerPool.Get();
            client.WorldId = worldId;
            writer.Put(PacketIds.S2C.JoinWorldOk);

            client.Send(writer);
            _writerPool.Return(writer);
        }

        private void HandleLeaveWorld(NetworkClient client, NetDataReader reader)
        {
            var worldId = reader.GetInt();
            var writer = _writerPool.Get();
            if (client.WorldId == worldId)
            {
                writer.Put(PacketIds.S2C.LeaveWorldOk);
            }
            else
            {
                writer.Put(PacketIds.S2C.LeaveWorldFailed);
            }
            client.Send(writer);
            _writerPool.Return(writer);
        }

        private void HandleChat(NetworkClient client, NetDataReader reader)
        {
            var chatMessage = reader.GetString();
            var clientWorldId = client.WorldId;
            var clientsInSameWorld = _clients
                .Where(c => c.WorldId == clientWorldId && c.Id != client.Id)
                .ToArray();
            if (clientsInSameWorld.Any())
            {
                var writer = _writerPool.Get();
                writer.Put(PacketIds.S2C.Chat);
                writer.Put(chatMessage);
                foreach (var clientInSameWorld in clientsInSameWorld)
                {
                    clientInSameWorld.Send(writer);
                }
                _writerPool.Return(writer);
            }
        }
    }
}
