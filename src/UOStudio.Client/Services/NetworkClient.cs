using System;
using System.Net;
using LiteNetLib;
using LiteNetLib.Layers;
using LiteNetLib.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Xna.Framework;
using Serilog;
using UOStudio.Common.Network;

namespace UOStudio.Client.Services
{
    public class NetworkClient : INetworkClient
    {
        private readonly ILogger _logger;
        private readonly IContext _context;
        private readonly string _apiEndpoint;

        private readonly NetManager _client;
        private readonly NetDataWriter _dataWriter;
        private NetPeer _server;

        private bool _isConnected;

        public NetworkClient(
            ILogger logger,
            IConfiguration configuration,
            IContext context,
            IProjectService projectService)
        {
            _logger = logger.ForContext<NetworkClient>();
            _context = context;
            _apiEndpoint = configuration["Api:ApiEndpoint"];

            var listener = new EventBasedNetListener();
            listener.PeerConnectedEvent += ListenerOnPeerConnectedEvent;
            listener.PeerDisconnectedEvent += ListenerOnPeerDisconnectedEvent;
            listener.NetworkReceiveEvent += ListenerOnNetworkReceiveEvent;
            listener.NetworkReceiveUnconnectedEvent += ListenerOnNetworkReceiveUnconnectedEvent;

            _dataWriter = new NetDataWriter(true);
            _client = new NetManager(listener, new XorEncryptLayer("UOStudio"));
        }

        public event Action<NetPeer> Connected;

        public event Action Disconnected;

        public event Action<ChunkData> ChunkReceived;

        public void Connect(string hostname, int port)
        {
            if (_client.IsRunning)
            {
                _client.Stop();
            }

            if (_client.Start())
            {
                _logger.Debug("NetworkClient - Running");
            }
            else
            {
                _logger.Error("NetworkClient - Unable to run");
            }

            _logger.Debug("NetworkClient: Contacting server {@HostName}:{@Port}", hostname, port);
            _client.Connect(hostname, port, "UOStudio");
        }

        public void Disconnect()
        {
            _client.Stop();
        }

        public void Update()
        {
            _client.PollEvents();
        }

        public void RequestChunk(int worldId, Point chunkPosition)
        {
            if (!_isConnected)
            {
                _logger.Debug("NetworkClient: Not connected");
                return;
            }

            _logger.Debug("NetworkClient: Requesting Chunk {@ChunkPositionX},{@ChunkPositionY}", chunkPosition.X, chunkPosition.Y);
            _dataWriter.Reset();
            _dataWriter.Put(PacketIds.C2S.RequestChunk);
            _dataWriter.Put(worldId); // worldId
            _dataWriter.Put(chunkPosition.X);
            _dataWriter.Put(chunkPosition.Y);

            _server.Send(_dataWriter, DeliveryMethod.ReliableOrdered);
        }

        private void ListenerOnPeerConnectedEvent(NetPeer peer)
        {
            _logger.Debug("NetworkClient - Connected to server {@Id}", peer.Id);
            _server = peer;
            _isConnected = true;
        }

        private void ListenerOnPeerDisconnectedEvent(
            NetPeer peer,
            DisconnectInfo disconnectInfo
        )
        {
            _isConnected = false;
        }

        private void ListenerOnNetworkReceiveEvent(
            NetPeer peer,
            NetDataReader reader,
            DeliveryMethod deliveryMethod)
        {
            var packetId = reader.GetInt();
            switch (packetId)
            {
                case PacketIds.S2C.RequestedChunk:
                    HandleChunkReceived(reader);
                    break;
            }
        }

        private void ListenerOnNetworkReceiveUnconnectedEvent(
            IPEndPoint remoteEndpoint,
            NetDataReader netDataReader,
            UnconnectedMessageType unconnectedMessageType
        )
        {
            // add later
        }

        private void HandleChunkReceived(NetDataReader reader)
        {
            var worldId = reader.GetInt();
            var chunkPositionX = reader.GetInt();
            var chunkPositionY = reader.GetInt();
            var chunkSize = reader.GetInt();
            var chunkLength = chunkSize * chunkSize;

            var chunkStaticTileData = new ChunkStaticTileData[chunkLength];
            for (var i = 0; i < chunkLength; i++)
            {
                var tileId = reader.GetUShort();
                var z = reader.GetInt();
                var hue = reader.GetInt();
                chunkStaticTileData[i] = new ChunkStaticTileData(tileId, z, hue);
            }

            var chunkItemTileData = new ChunkItemTileData[chunkLength];
            for (var i = 0; i < chunkLength; i++)
            {
                var tileId = reader.GetUShort();
                var z = reader.GetInt();
                var hue = reader.GetInt();
                chunkItemTileData[i] = new ChunkItemTileData(tileId, z, hue);
            }

            var chunkData = new ChunkData(worldId, new Point(chunkPositionX, chunkPositionY), chunkStaticTileData, chunkItemTileData);
            var chunkReceived = ChunkReceived;
            chunkReceived?.Invoke(chunkData);
        }
    }
}
