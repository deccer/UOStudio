using System.Collections.Generic;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using Serilog;
using UOStudio.Server.Core;
using UOStudio.Server.Core.Settings;
using UOStudio.Server.Network.PacketHandlers;

namespace UOStudio.Server.Network
{
    public class NetworkServer : INetworkServer
    {
        private readonly ILogger _logger;
        private readonly IAppSettingsProvider _appSettingsProvider;
        private readonly IPacketProcessor _packetProcessor;
        private readonly EventBasedNetListener _listener;
        private readonly NetManager _server;
        private readonly NetworkSession _networkSession;

        public NetworkServer(
            ILogger logger,
            IAppSettingsProvider appSettingsProvider,
            IPacketProcessor packetProcessor)
        {
            _logger = logger;
            _appSettingsProvider = appSettingsProvider;
            _packetProcessor = packetProcessor;
            _networkSession = new NetworkSession();
            _listener = new EventBasedNetListener();
            _server = new NetManager(_listener);
        }

        private void Initialize()
        {
            _server.Start(_appSettingsProvider.AppSettings.Network.Port);

            _listener.ConnectionRequestEvent += ConnectionRequestEventHandler;
            _listener.PeerConnectedEvent += PeerConnectedEventHandler;
            _listener.NetworkReceiveEvent += NetworkReceiveEventHandler;
        }

        private void NetworkReceiveEventHandler(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var packetId = reader.GetInt();
            _logger.Debug($"Packet: {packetId}");
            switch (packetId)
            {
                case 1:
                    {
                        var clientConnectPacket = new ClientConnectPacket(reader);
                        var clientConnected = _packetProcessor.Process<ClientConnectPacket, int>(clientConnectPacket);

                        //_networkSession.AddActiveAccount(peer.Id, account);

                        _logger.Debug($"Packet - Login: {clientConnectPacket.UserName}");
                        break;

                    }
            }
        }

        public void Run()
        {
            Initialize();
            while (_server.IsRunning)
            {
                _server.PollEvents();
                Thread.Sleep(15);
            }

            _server.Stop();
        }

        private void ConnectionRequestEventHandler(ConnectionRequest request)
        {
            if (_server.ConnectedPeersCount < _appSettingsProvider.AppSettings.Network.ConnectedPeersCount)
            {
                request.AcceptIfKey("UOStudio");
            }
            else
            {
                request.Reject();
            }
        }

        private void PeerConnectedEventHandler(NetPeer peer)
        {
            _logger.Information("Server - Client Connected: {0}", peer.EndPoint);

            var writer = new NetDataWriter();
            writer.Put("Hello you majestic UO admirer!");
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }
    }

    public class NetworkSession
    {
        public IDictionary<int, Account> AccountsOnline { get; }

        public NetworkSession()
        {
            AccountsOnline = new Dictionary<int, Account>();
        }

        public void AddActiveAccount(int userId, Account account)
        {
            AccountsOnline.Add(userId, account);
        }
    }
}
