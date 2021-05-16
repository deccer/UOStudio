using System;
using System.Net;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Layers;
using LiteNetLib.Utils;
using Microsoft.Extensions.Configuration;
using Serilog;

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

        public async Task ConnectAsync()
        {
            // add later
        }

        public void Disconnect()
        {
            _client.Stop();
        }

        public void Update()
        {
            _client.PollEvents();
        }

        private void StartClient()
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

            _client.Connect("servername", 0, "UOStudio");
        }

        private void ListenerOnPeerConnectedEvent(NetPeer peer)
        {
            _logger.Debug("NetworkClient - Connected to server {@Id}", peer.Id);
            _server = peer;
        }

        private void ListenerOnPeerDisconnectedEvent(
            NetPeer peer,
            DisconnectInfo disconnectInfo
        )
        {
            // add later
        }

        private void ListenerOnNetworkReceiveEvent(
            NetPeer peer,
            NetDataReader reader,
            DeliveryMethod deliveryMethod)
        {
            var packetId = reader.GetInt();
        }

        private void ListenerOnNetworkReceiveUnconnectedEvent(
            IPEndPoint remoteEndpoint,
            NetDataReader netDataReader,
            UnconnectedMessageType unconnectedMessageType
        )
        {
            // add later
        }
    }
}
