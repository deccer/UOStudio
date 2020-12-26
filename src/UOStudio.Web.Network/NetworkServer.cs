using System;
using System.Collections.Generic;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using Serilog;
using UOStudio.Core.Network;
using UOStudio.Shared.Network;
using UOStudio.Web.Services;

namespace UOStudio.Web.Network
{
    public class NetworkServer : INetworkServer
    {
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly ILogger _logger;
        private readonly NetManager _server;
        private readonly Thread _clientThread;
        private readonly CancellationTokenSource _cancellationToken;
        private readonly IList<NetworkServerClient> _clients;

        public NetworkServer(ILogger logger, IUserService userService, IProjectService projectService)
        {
            _logger = logger.ForContext<NetworkServer>();
            _userService = userService;
            _projectService = projectService;

            var listener = new EventBasedNetListener();
            listener.NetworkReceiveEvent += ListenerOnNetworkReceiveEvent;
            listener.ConnectionRequestEvent += ListenerOnConnectionRequestEvent;

            _server = new NetManager(listener);
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
            _logger.Debug("Starting NetworkServer...");
            _server.Start(port);
            _clientThread.Start();
            _logger.Debug($"Starting NetworkServer...Done. Running on port {port}");
        }

        public void Stop()
        {
            _logger.Debug("Stopping NetworkServer...");
            _cancellationToken.Cancel();
            _server.Stop();
            _logger.Debug("Stopping NetworkServer...Done.");
        }

        private void ClientThreadProc(object state)
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                _server.PollEvents();
            }
        }

        private void ListenerOnConnectionRequestEvent(ConnectionRequest request)
        {
            request.AcceptIfKey("UOStudio");
        }

        private void ListenerOnNetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var requestId = (RequestIds.C2S)reader.GetInt();
            _logger.Debug($"Request: {requestId}");
            switch (requestId)
            {
                case RequestIds.C2S.ChatMessage:
                    HandleChatMessage(peer, reader);
                    break;
                case RequestIds.C2S.Connect:
                    HandleClientConnect(peer, reader);
                    break;
                case RequestIds.C2S.Disconnect:
                    HandleClientDisconnect(peer, reader);
                    break;
                case RequestIds.C2S.JoinProject:
                    HandleJoinProject(peer, reader);
                    break;
                case RequestIds.C2S.LeaveProject:
                    HandleLeaveProject(peer, reader);
                    break;
            }
        }

        private void HandleChatMessage(NetPeer peer, NetPacketReader reader)
        {
            throw new System.NotImplementedException();
        }

        private void HandleLeaveProject(NetPeer peer, NetPacketReader reader)
        {
            throw new System.NotImplementedException();
        }

        private void HandleJoinProject(NetPeer peer, NetDataReader reader)
        {
            var userId = Guid.ParseExact(reader.GetString(), "N");
            var projectId = Guid.ParseExact(reader.GetString(), "N");
            var projectHash = new byte[16];
            reader.GetBytes(projectHash, projectHash.Length);

            var result = _projectService
                .JoinProjectAsync(projectId, userId, projectHash)
                .GetAwaiter()
                .GetResult();

            var writer = new NetDataWriter();
            if (result.IsSuccess)
            {
                var joinResult = result.Value;
                writer.Put((int)RequestIds.S2C.JoinProjectSuccess);
                writer.Put(projectId.ToString("N"));
                writer.Put((int)joinResult);
            }
            else
            {
                writer.Put((int)RequestIds.S2C.JoinProjectFailed);
                writer.Put(result.Error);
            }

            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        private void HandleClientDisconnect(NetPeer peer, NetPacketReader reader)
        {
            throw new System.NotImplementedException();
        }

        private async void HandleClientConnect(NetPeer peer, NetDataReader reader)
        {
            var writer = new NetDataWriter();
            var userName = reader.GetString();
            var password = reader.GetString();

            var user = await _userService.GetUserByNameAndVerifyPasswordAsync(userName, password);
            if (user == null)
            {
                writer.Put((int)RequestIds.S2C.ConnectFailed);
                writer.Put("TODO");

                peer.Send(writer, DeliveryMethod.ReliableOrdered);
                return;
            }

            if (user.IsBlocked)
            {
                writer.Put((int)RequestIds.S2C.ConnectFailed);
                writer.Put("TODO");

                peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }

            writer.Put((int)RequestIds.S2C.ConnectSuccess);
            writer.Put(user.Id.ToString("N"));
            peer.Send(writer, DeliveryMethod.ReliableOrdered);

            _clients.Add(new NetworkServerClient(peer, user.Id));
        }
    }
}
