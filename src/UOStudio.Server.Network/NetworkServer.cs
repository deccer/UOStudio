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
        private readonly IRequestProcessor _requestProcessor;
        private readonly EventBasedNetListener _listener;
        private readonly NetManager _server;

        public NetworkServer(
            ILogger logger,
            IAppSettingsProvider appSettingsProvider,
            IRequestProcessor requestProcessor)
        {
            _logger = logger;
            _appSettingsProvider = appSettingsProvider;
            _requestProcessor = requestProcessor;
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
                case PacketIds.C2S.ChatMessage:
                    await HandleChatMessage(peer, reader);
                    break;
                case PacketIds.C2S.Connect:
                    await HandleClientConnect(peer, reader);
                    break;
                case PacketIds.C2S.Disconnect:
                    break;
                case PacketIds.C2S.CreateAccount:
                    break;
                case PacketIds.C2S.DeleteAccount:
                    break;
                case PacketIds.C2S.UpdateAccount:
                    break;
                case PacketIds.C2S.ListAccounts:
                    break;
                case PacketIds.C2S.ListProjects:
                    break;
                case PacketIds.C2S.CreateProject:
                    await HandleCreateProject(peer, reader);
                    break;
                case PacketIds.C2S.DeleteProject:
                    break;
                case PacketIds.C2S.UpdateProject:
                    break;
                case PacketIds.C2S.JoinProject:
                    break;
                case PacketIds.C2S.LeaveProject:
                    break;
            }
        }

        private async Task HandleCreateProject(NetPeer peer, NetDataReader reader)
        {
            var createProjectRequest = new CreateProjectRequest(reader);
            var writer = new NetDataWriter();
            var (isSuccess, _, value, error) = await _requestProcessor.Process<CreateProjectRequest, CreateProjectResult>(createProjectRequest);
            if (isSuccess)
            {
                writer.Put(PacketIds.S2C.CreateProjectSuccess);
                writer.Put(value.ProjectId);
            }
            else
            {
                writer.Put(PacketIds.S2C.CreateProjectFailed);
                writer.Put(error);
            }

            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        private async Task HandleChatMessage(NetPeer peer, NetDataReader reader)
        {

        }

        private async Task HandleClientConnect(NetPeer peer, NetDataReader reader)
        {
            var clientConnectRequest = new ClientConnectRequest(reader);
            var writer = new NetDataWriter();
            var (isSuccess, _, value, error) = await _requestProcessor.Process<ClientConnectRequest, ClientConnectResult>(clientConnectRequest);
            if (isSuccess)
            {
                _logger.Debug($"Packet - Login: {clientConnectRequest.UserName}");
                LoginSuccess?.Invoke(peer, value.AccountId, value.Projects);

                var permissions = 0;
                writer.Put(PacketIds.S2C.ConnectSuccess);
                writer.Put(value.AccountId.ToString("N"));
                writer.Put(permissions);
                var projectCount = value.Projects.Count;
                writer.Put(projectCount);
                foreach (var project in value.Projects)
                {
                    writer.Put(project.Id);
                    writer.Put(project.Name);
                    writer.Put(project.Description);
                    writer.Put(project.ClientVersion);
                }
            }
            else
            {
                LoginFailure?.Invoke(peer, error);

                writer.Put(PacketIds.S2C.ConnectFailed);
                writer.Put(error);
            }
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
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
