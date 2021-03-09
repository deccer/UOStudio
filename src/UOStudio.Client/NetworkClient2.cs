using System;
using System.Collections.Generic;
using System.Net;
using LiteNetLib;
using LiteNetLib.Layers;
using LiteNetLib.Utils;
using Serilog;
using UOStudio.Common.Contracts;
using UOStudio.Common.Network;

namespace UOStudio.Client
{
    internal class NetworkClient2 : INetworkClient
    {
        private readonly ILogger _logger;
        private readonly NetManager _client;
        private Profile _profile;

        private bool _isLoggedIn;
        private int _userId;
        private NetPeer _server;

        private readonly NetDataWriter _dataWriter;

        public NetworkClient2(
            ILogger logger)
        {
            _logger = logger.ForContext<NetworkClient2>();

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

        public event Action<int, IReadOnlyCollection<ProjectDto>> LoginSucceeded;

        public event Action<string> LoginFailed;

        public event Action<ProjectDetailDto> GetProjectDetailsSucceeded;

        public event Action<string> GetProjectDetailsFailed;

        public void Connect(Profile profile)
        {
            _profile = profile;
            if (_client.IsRunning)
            {
                _client.Stop();
            }

            if (_client.Start())
            {
                _logger.Debug("Network started");
            }
            else
            {
                _logger.Error("Unable to start network");
            }

            _client.Connect(_profile.ServerName, _profile.ServerPort, "UOStudio");
        }

        public void Disconnect()
        {
            _client.Stop();
        }

        public void Update()
        {
            _client.PollEvents();
        }

        public void GetProjectDetailsByProjectId(int projectId)
        {
            _logger.Debug("NetworkClient - GetProjectDetailsByProjectId({@ProjectId})", projectId);
            _dataWriter.Reset();
            _dataWriter.Put(PacketIds.C2S.GetProjectDetailsByProjectId);
            _dataWriter.Put(_userId);
            _dataWriter.Put(projectId);
            _server.Send(_dataWriter, DeliveryMethod.ReliableOrdered);
        }

        public void GetProjectDetailsByProjectName(string projectName)
        {
            _logger.Debug("NetworkClient - GetProjectDetailsByProjectName({@ProjectName})", projectName);
            _dataWriter.Reset();
            _dataWriter.Put(PacketIds.C2S.GetProjectDetailsByProjectName);
            _dataWriter.Put(_userId);
            _dataWriter.Put(projectName);
            _server.Send(_dataWriter, DeliveryMethod.ReliableOrdered);
        }

        private void HandleLoginOkPacket(NetDataReader reader)
        {
            _logger.Debug("NetworkClient - HandleLoginOk");
            var projects = new List<ProjectDto>();
            var userId = reader.GetInt();
            var projectCount = reader.GetInt();
            for (var i = 0; i < projectCount; i++)
            {
                var projectId = reader.GetInt();
                var projectName = reader.GetString();

                projects.Add(new ProjectDto
                {
                    Id = projectId,
                    Name = projectName
                });
            }

            var loginSucceeded = LoginSucceeded;
            loginSucceeded?.Invoke(userId, projects);
            _isLoggedIn = true;
            _userId = userId;
        }

        private void HandleLoginErrorPacket(NetDataReader reader)
        {
            var errorMessage = reader.GetString();

            _logger.Error("NetworkClient - HandleLoginError: {@ErrorMessage}", errorMessage);
            var loginFailed = LoginFailed;
            loginFailed?.Invoke(errorMessage);
        }

        private void HandleUserLogoutPacket(NetDataReader reader)
        {
            _logger.Debug("NetworkClient - HandleUserLogout");
            var userId = reader.GetInt();
        }

        private void HandleGetProjectDetailsOk(NetDataReader reader)
        {
            _logger.Debug("NetworkClient - HandleGetProjectDetailsOk");

            var projectName = reader.GetString();
            var projectDescription = reader.GetString();
            var projectTemplate = reader.GetString();
            var clientVersion = reader.GetString();
            var createdAt = reader.GetLong();
            var createAtOffset = reader.GetLong();
            var createdBy = reader.GetString();

            var projectDetails = new ProjectDetailDto
            {
                Name = projectName,
                Description = projectDescription,
                Template = projectTemplate,
                ClientVersion = clientVersion,
                CreatedAt = new DateTimeOffset(createdAt, TimeSpan.FromTicks(createAtOffset)),
                CreatedBy = createdBy
            };

            var getProjectDetailsOk = GetProjectDetailsSucceeded;
            getProjectDetailsOk?.Invoke(projectDetails);
        }

        private void HandleGetProjectDetailsFailed(NetDataReader reader)
        {
            var errorMessage = reader.GetString();
            _logger.Error("NetworkClient - HandleGetProjectDetailsFailed: {@ErrorMessage}", errorMessage);
            var getProjectDetailsFailed = GetProjectDetailsFailed;
            getProjectDetailsFailed?.Invoke(errorMessage);
        }

        private void ListenerOnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            var disconnected = Disconnected;
            disconnected?.Invoke();

            _isLoggedIn = false;
            _userId = -1;
            _logger.Debug("NetworkClient - Disconnected from server {@Id}", peer.Id);
        }

        private void ListenerOnPeerConnectedEvent(NetPeer peer)
        {
            _server = peer;
            var connected = Connected;
            connected?.Invoke(_server);

            _logger.Debug("NetworkClient - Connected to server {@Id}", peer.Id);
            _logger.Debug("NetworkClient - Logging in with profile {@Profile}", _profile.Name);

            _dataWriter.Reset();
            _dataWriter.Put(PacketIds.C2S.Login);
            _dataWriter.Put(_profile.UserName);
            _dataWriter.Put(_profile.Password);
            _server.Send(_dataWriter, DeliveryMethod.ReliableOrdered);
        }

        private void ListenerOnNetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var packetId = reader.GetInt();
            switch (packetId)
            {
                case PacketIds.S2C.LoginOk:
                    HandleLoginOkPacket(reader);
                    break;
                case PacketIds.S2C.LoginError:
                    HandleLoginErrorPacket(reader);
                    break;
                case PacketIds.S2C.Broadcast.Logout:
                    HandleUserLogoutPacket(reader);
                    break;
                case PacketIds.S2C.GetProjectDetailsOk:
                    HandleGetProjectDetailsOk(reader);
                    break;
                case PacketIds.S2C.GetProjectDetailsError:
                    HandleGetProjectDetailsFailed(reader);
                    break;
            }
        }

        private void ListenerOnNetworkReceiveUnconnectedEvent(IPEndPoint remoteEndpoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            _logger.Debug("NetworkClient - Receive unconnected event");
        }
    }
}
