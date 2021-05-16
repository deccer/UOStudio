using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Layers;
using LiteNetLib.Utils;
using MediatR;
using Microsoft.Extensions.ObjectPool;
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
        private readonly IList<NetworkServerClient> _clients;

        private readonly ObjectPool<NetDataWriter> _writerPool;

        public NetworkServer(
            ILogger logger,
            ServerSettings serverSettings,
            IMediator mediator)
        {
            _logger = logger;
            _serverSettings = serverSettings;
            _mediator = mediator;

            _writerPool = new DefaultObjectPool<NetDataWriter>(new DefaultPooledObjectPolicy<NetDataWriter>());

            var listener = new EventBasedNetListener();
            listener.ConnectionRequestEvent += ListenerOnConnectionRequestEvent;
            listener.PeerConnectedEvent += ListenerOnPeerConnectedEvent;
            listener.PeerDisconnectedEvent += ListenerOnPeerDisconnectedEvent;
            listener.NetworkReceiveEvent += async (peer, reader, deliveryMethod) => await ListenerOnNetworkReceiveEvent(peer, reader, deliveryMethod);

            _server = new NetManager(listener, new XorEncryptLayer("UOStudio"));
            _clients = new List<NetworkServerClient>(16);
            _clientThread = new Thread(ClientThreadProc);
            _cancellationToken = new CancellationTokenSource();
        }

        public void Dispose()
        {
            _cancellationToken?.Dispose();
        }

        public void Start(int port)
        {
            _logger.Debug("NetworkServer - Starting...");
            _server.Start(port);
            _clientThread.Start();
            _logger.Debug("NetworkServer - Starting...Done. Running on port {@Port}", port);
        }

        public void Stop()
        {
            _logger.Debug("NetworkServer - Stopping...");
            _cancellationToken.Cancel();
            _server.Stop();
            _logger.Debug("NetworkServer - Stopping...Done");
        }

        private async Task ListenerOnNetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var packetId = reader.GetInt();
            switch (packetId)
            {
                case PacketIds.C2S.JoinProject:
                    break;
                case PacketIds.C2S.LeaveProject:
                    break;
            }
        }

        private void ListenerOnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            _logger.Information("NetworkServer - Client {@Id} disconnected", peer.Id);
        }

        private void ListenerOnPeerConnectedEvent(NetPeer peer)
        {
            _logger.Information("NetworkServer - Client {@Id} connected", peer.Id);
        }

        private void ListenerOnConnectionRequestEvent(ConnectionRequest connectionRequest)
        {
            var remoteEndpoint = connectionRequest.RemoteEndPoint.ToString();
            _logger.Debug("NetworkServer - Connection request from {@RemoteEndPoint}", remoteEndpoint);
            if (_server.ConnectedPeersCount < _serverSettings.MaximumConnectedPeersCount)
            {
                connectionRequest.AcceptIfKey("UOStudio");
                _logger.Debug("NetworkServer - Connection request from {@RemoteEndPoint} accepted", remoteEndpoint);
            }
            else
            {
                connectionRequest.Reject();
                _logger.Debug("NetworkServer - Connection request from {@RemoteEndPoint} rejected", remoteEndpoint);
            }
        }

        private void ClientThreadProc(object state)
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                _server.PollEvents();
            }
        }
    }
}
