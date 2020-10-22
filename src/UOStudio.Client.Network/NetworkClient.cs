using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using Serilog;

namespace UOStudio.Client.Network
{
    public class NetworkClient
        : INetworkClient
    {
        private readonly ILogger _logger;
        private readonly EventBasedNetListener _listener;
        private readonly NetManager _client;
        private NetPeer _peerConnection;
        private readonly Thread _clientThread;

        public event Action<EndPoint, int> Connected;

        public NetworkClient(ILogger logger)
        {
            _logger = logger;
            _listener = new EventBasedNetListener();
            _client = new NetManager(_listener);
            _clientThread = new Thread(ClientThreadProc);

            _listener.DeliveryEvent += DeliveryEventHandler;
            _listener.ConnectionRequestEvent += ConnectionRequestEventHandler;
            _listener.NetworkErrorEvent += NetworkErrorEventHandler;
            _listener.NetworkLatencyUpdateEvent += NetworkLatencyUpdateEventHandler;
            _listener.NetworkReceiveEvent += NetworkReceiveEventHandler;
            _listener.NetworkReceiveUnconnectedEvent += NetworkReceiveUnconnectedEventHandler;
            _listener.PeerConnectedEvent += PeerConnectedEventHandler;
            _listener.PeerDisconnectedEvent += PeerDisconnectedEventHandler;
        }

        private void NetworkReceiveUnconnectedEventHandler(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            _logger.Debug($"NetworkClient - UDP - {remoteEndPoint} {messageType}");
        }

        private void NetworkLatencyUpdateEventHandler(NetPeer peer, int latency)
        {
            _logger.Debug($"NetworkClient - LatencyUpdate - {latency}ms");
        }

        private void NetworkErrorEventHandler(IPEndPoint endpoint, SocketError socketError)
        {
            _logger.Error($"NetworkClient - Error - {socketError}");
        }

        private void ConnectionRequestEventHandler(ConnectionRequest request)
        {
            _logger.Debug($"NetworkClient - ConnectionRequest - {request.RemoteEndPoint}");
        }

        private void DeliveryEventHandler(NetPeer peer, object userdata)
        {
            _logger.Debug($"NetworkClient - Delivery {peer.Id}");
        }

        private void ClientThreadProc(object? obj)
        {
            while (_client.IsRunning)
            {
                _client.PollEvents();
                Thread.Sleep(15);
            }

            _client.Stop();
        }

        private void PeerDisconnectedEventHandler(NetPeer peer, DisconnectInfo disconnectinfo)
        {
            _logger.Debug($"NetworkClient - Disconnected: {peer.EndPoint}");
        }

        private void PeerConnectedEventHandler(NetPeer peer)
        {
            _logger.Debug($"NetworkClient - Connected to {peer.EndPoint}");
            Connected?.Invoke(peer.EndPoint, peer.Id);
        }

        public void Connect(string address, int port)
        {
            _logger.Debug($"NetworkClient - Connecting to {address}:{port}...");
            _client.Start();
            _clientThread.Start();
            _peerConnection = _client.Connect(address, port, "UOStudio");
        }

        public void Disconnect()
        {
            _client.DisconnectPeer(_peerConnection);
        }

        public void SendMessage(string message)
        {
            var netWriter = new NetDataWriter();
            netWriter.Put(message);
            _peerConnection.Send(netWriter, DeliveryMethod.ReliableOrdered);
        }

        private void NetworkReceiveEventHandler(NetPeer peer, NetPacketReader dataReader, DeliveryMethod deliveryMethod)
        {
            _logger.Information("NetworkClient - Server - '{0}'", dataReader.GetString(128));
            dataReader.Recycle();
        }
    }
}
