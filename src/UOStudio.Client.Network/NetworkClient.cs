using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using Serilog;
using UOStudio.Client.Core;
using UOStudio.Client.Network.Packets;
using UOStudio.Core.Network;

namespace UOStudio.Client.Network
{
    public class NetworkClient : INetworkClient
    {
        private readonly ILogger _logger;
        private readonly EventBasedNetListener _listener;
        private readonly NetManager _client;
        private NetPeer _peerConnection;
        private readonly Thread _clientThread;
        private readonly NetDataWriter _dataWriter;

        private Profile _profile;

        public event Action<EndPoint, int> Connected;

        public event Action Disconnected;

        public event Action<Guid, int, IList<Project>> LoginSuccessful;

        public event Action<string> LoginFailed;

        public bool IsConnected { get; private set; }

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

            _dataWriter = new NetDataWriter(false);
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
            Disconnected?.Invoke();
            IsConnected = false;
        }

        private void PeerConnectedEventHandler(NetPeer peer)
        {
            _logger.Debug($"NetworkClient - Connected to {peer.EndPoint}");
            Connected?.Invoke(peer.EndPoint, peer.Id);
            IsConnected = true;

            var clientConnectPacket = new ClientConnectPacket(_dataWriter, _profile);
            clientConnectPacket.Send(peer);
        }

        public void Connect(Profile profile)
        {
            _profile = profile;
            _logger.Debug($"NetworkClient - Connecting to {_profile.ServerName}:{_profile.ServerPort}...");
            if (IsConnected)
            {
                _clientThread.Abort();
                _client.Stop();
            }
            _client.Start();
            _clientThread.Start();

            _peerConnection = _client.Connect(_profile.ServerName, _profile.ServerPort, "UOStudio");
        }

        public void Disconnect()
        {
            if (IsConnected)
            {
                _client.DisconnectPeer(_peerConnection);
            }
        }

        public void SendMessage(Guid accountId, string message)
        {
            var netWriter = new NetDataWriter();
            netWriter.Put(PacketIds.C2S.ChatMessage);
            netWriter.Put(accountId.ToString("N"));
            netWriter.Put(message);
            _peerConnection.Send(netWriter, DeliveryMethod.ReliableOrdered);
        }

        private void NetworkReceiveEventHandler(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var packetId = reader.GetInt();
            _logger.Debug($"Packet: {packetId}");
            switch (packetId)
            {
                case PacketIds.S2C.ConnectSuccess:
                    HandleConnectSuccess(peer, reader);
                    break;
                case PacketIds.S2C.ConnectFailed:
                    HandleConnectFailed(peer, reader);
                    break;
                case PacketIds.S2C.SystemMessage:
                    HandleSystemMessage(peer, reader);
                    break;
                case PacketIds.S2C.ChatMessage:
                    HandleChatMessage(peer, reader);
                    break;
                case PacketIds.S2C.RefreshUsers:
                    HandleRefreshUsers(peer, reader);
                    break;
                case PacketIds.S2C.ListAccountsSuccess:
                    HandleListAccountsSuccess(peer, reader);
                    break;
                case PacketIds.S2C.ListAccountsFailed:
                    HandleListAccountsFailed(peer, reader);
                    break;
                case PacketIds.S2C.CreateAccountSuccess:
                    HandleCreateAccountSuccess(peer, reader);
                    break;
                case PacketIds.S2C.CreateAccountFailed:
                    HandleCreateAccountFailed(peer, reader);
                    break;
                case PacketIds.S2C.DeleteAccountSuccess:
                    HandleDeleteAccountSuccess(peer, reader);
                    break;
                case PacketIds.S2C.DeleteAccountFailed:
                    HandleDeleteAccountFailed(peer, reader);
                    break;
                case PacketIds.S2C.UpdateAccountSuccess:
                    HandleUpdateAccountSuccess(peer, reader);
                    break;
                case PacketIds.S2C.UpdateAccountFailed:
                    HandleUpdateAccountFailed(peer, reader);
                    break;
                case PacketIds.S2C.ListProjectsSuccess:
                    HandleListProjectSuccess(peer, reader);
                    break;
                case PacketIds.S2C.ListProjectsFailed:
                    HandleListProjectFailed(peer, reader);
                    break;
                case PacketIds.S2C.CreateProjectSuccess:
                    HandleCreateProjectSuccess(peer, reader);
                    break;
                case PacketIds.S2C.CreateProjectFailed:
                    HandleCreateProjectFailed(peer, reader);
                    break;
                case PacketIds.S2C.DeleteProjectSuccess:
                    HandleDeleteProjectSuccess(peer, reader);
                    break;
                case PacketIds.S2C.DeleteProjectFailed:
                    HandleDeleteProjectFailed(peer, reader);
                    break;
                case PacketIds.S2C.UpdateProjectSuccess:
                    HandleUpdateProjectSuccess(peer, reader);
                    break;
                case PacketIds.S2C.UpdateProjectFailed:
                    HandleUpdateProjectFailed(peer, reader);
                    break;
            }

            reader.Recycle();
        }

        private void HandleUpdateProjectFailed(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private void HandleUpdateProjectSuccess(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private void HandleDeleteProjectFailed(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private void HandleDeleteProjectSuccess(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private void HandleCreateProjectFailed(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private void HandleCreateProjectSuccess(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private void HandleListProjectFailed(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private void HandleListProjectSuccess(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private void HandleUpdateAccountFailed(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private void HandleUpdateAccountSuccess(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private void HandleDeleteAccountFailed(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private void HandleDeleteAccountSuccess(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private void HandleCreateAccountFailed(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private void HandleCreateAccountSuccess(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private void HandleListAccountsFailed(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private void HandleListAccountsSuccess(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private void HandleRefreshUsers(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private void HandleChatMessage(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private void HandleSystemMessage(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private void HandleConnectSuccess(NetPeer peer, NetDataReader reader)
        {
            var accountId = Guid.Parse(reader.GetString());
            var permissions = reader.GetInt();
            var projectCount = reader.GetInt();
            var projects = new List<Project>();
            for (var i = 0; i < projectCount; ++i)
            {
                var project = new Project
                {
                    Id = reader.GetInt(),
                    Name = reader.GetString(),
                    Description = reader.GetString(),
                    ClientVersion = reader.GetString()
                };
                projects.Add(project);
            }
            var loginSuccessful = LoginSuccessful;
            loginSuccessful?.Invoke(accountId, permissions, projects);
        }

        private void HandleConnectFailed(NetPeer peer, NetDataReader reader)
        {
            var loginFailed = LoginFailed;
            loginFailed?.Invoke(reader.GetString());
        }
    }
}
