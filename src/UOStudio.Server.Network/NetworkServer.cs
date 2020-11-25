using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using LiteNetLib;
using LiteNetLib.Utils;
using Serilog;
using UOStudio.Core.Network;
using UOStudio.Server.Core;
using UOStudio.Server.Core.Settings;
using UOStudio.Server.Data;
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

        public NetworkServer(
            ILogger logger,
            IAppSettingsProvider appSettingsProvider,
            IPacketProcessor packetProcessor)
        {
            _logger = logger;
            _appSettingsProvider = appSettingsProvider;
            _packetProcessor = packetProcessor;
            _listener = new EventBasedNetListener();
            _server = new NetManager(_listener);
        }

        public event Action<NetPeer, Guid, IList<Project>> LoginSuccess;

        public event Action<NetPeer, string> LoginFailure;

        private void Initialize()
        {
            _server.Start(_appSettingsProvider.AppSettings.Network.Port);

            _listener.ConnectionRequestEvent += ConnectionRequestEventHandler;
            _listener.PeerConnectedEvent += PeerConnectedEventHandler;
            _listener.NetworkReceiveEvent += async (netPeer, netReader, deliveryMethod) => await NetworkReceiveEventHandler(netPeer, netReader, deliveryMethod);
        }

        private async Task NetworkReceiveEventHandler(NetPeer peer, NetDataReader reader, DeliveryMethod deliveryMethod)
        {
            var packetId = reader.GetInt();
            _logger.Debug($"Packet: {packetId}");
            switch (packetId)
            {
                case PacketIds.Connect:
                    {
                        await HandleClientConnect(peer, reader);
                        break;
                    }
                case PacketIds.Disconnect:
                    break;
                case PacketIds.CreateAccount:
                    break;
                case PacketIds.DeleteAccount:
                    break;
                case PacketIds.UpdateAccount:
                    break;
                case PacketIds.ListAccounts:
                    break;
                case PacketIds.ListProjects:
                    break;
                case PacketIds.CreateProject:
                    break;
                case PacketIds.DeleteProject:
                    break;
                case PacketIds.UpdateProject:
                    break;
                case PacketIds.JoinProject:
                    break;
                case PacketIds.LeaveProject:
                    break;

            }
        }

        private async Task HandleClientConnect(NetPeer peer, NetDataReader reader)
        {
            var clientConnectPacket = new ClientConnectRequest(reader);
            var (isSuccess, _, value, error) = await _packetProcessor.Process<ClientConnectRequest, ClientConnectResult>(clientConnectPacket);
            if (isSuccess)
            {
                _logger.Debug($"Packet - Login: {clientConnectPacket.UserName}");
                LoginSuccess?.Invoke(peer, value.AccountId, value.Projects);
            }
            else
            {
                LoginFailure?.Invoke(peer, error);
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
}
