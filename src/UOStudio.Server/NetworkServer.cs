using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using LiteNetLib;
using LiteNetLib.Layers;
using LiteNetLib.Utils;
using MediatR;
using Microsoft.Extensions.ObjectPool;
using Serilog;
using UOStudio.Common.Contracts;
using UOStudio.Common.Network;
using UOStudio.Server.Common;
using UOStudio.Server.Extensions;
using UOStudio.Server.Requests;

namespace UOStudio.Server
{
    internal class NetworkServer
    {
        private readonly ILogger _logger;
        private readonly ServerSettings _serverSettings;
        private readonly IMediator _mediator;
        private readonly NetManager _server;
        private bool _serverRunning = true;

        private readonly ObjectPool<NetDataWriter> _writerPool;

        public NetworkServer(
            ILogger logger,
            ServerSettings serverSettings,
            IMediator mediator)
        {
            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

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
        }

        private async Task ListenerOnNetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var packetId = reader.GetInt();
            switch (packetId)
            {
                case PacketIds.C2S.Login:
                    await HandleLoginPacketAsync(peer, reader);
                    break;
                case PacketIds.C2S.Logout:
                    await HandleLogoutPacketAsync(reader);
                    break;
                case PacketIds.C2S.CreateProject:
                    await HandleCreateProjectAsync(peer, reader);
                    break;
                case PacketIds.C2S.GetProjectDetailsByProjectId:
                    await HandleGetProjectDetailsByProjectIdAsync(peer, reader);
                    break;
                case PacketIds.C2S.GetProjectDetailsByProjectName:
                    await HandleGetProjectDetailsByProjectNameAsync(peer, reader);
                    break;
            }
        }

        private async Task HandleLoginPacketAsync(NetPeer peer, NetDataReader reader)
        {
            var clientLoginRequest = new ClientLoginRequest(reader);
            var clientLoginResult = await _mediator.Send(clientLoginRequest).ConfigureAwait(false);

            var writer = _writerPool.Get();
            writer.Reset();
            if (clientLoginResult.IsSuccess)
            {
                var userId = clientLoginResult.Value.UserId;
                var projects = clientLoginResult.Value.Projects;

                writer.Put(PacketIds.S2C.LoginOk);
                writer.Put(userId);
                writer.Put(projects.Count);
                foreach (var project in projects)
                {
                    writer.Put(project.Id);
                    writer.Put(project.Name);
                }
                peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
            else
            {
                writer.Put(PacketIds.S2C.LoginError);
                writer.Put(clientLoginResult.Error);
                peer.Disconnect(writer);
            }

            _writerPool.Return(writer);
        }

        private async Task HandleLogoutPacketAsync(NetDataReader reader)
        {
            var clientLogoutRequest = new ClientLogoutRequest(reader);
            var clientLogoutResult = await _mediator.Send(clientLogoutRequest).ConfigureAwait(false);

            _writerPool.AcquireAndRelease(clientLogoutResult, (result, writer) =>
            {
                if (result.IsSuccess)
                {
                    writer.Put(PacketIds.S2C.Broadcast.Logout);
                    writer.Put(clientLogoutRequest.UserId);

                    _server.SendToAll(writer, DeliveryMethod.ReliableOrdered);
                }
                else
                {
                    _logger.Error("NetworkServer - Unable to logout user {@Id}", clientLogoutRequest.UserId);
                }
            });
        }

        private Task HandleCreateProjectAsync(NetPeer peer, NetDataReader reader)
        {
            return Task.CompletedTask;
        }

        private async Task HandleGetProjectDetailsByProjectIdAsync(NetPeer peer, NetDataReader reader)
        {
            var getProjectDetailsRequest = new GetProjectDetailsByProjectIdRequest(reader);
            var getProjectDetailsResult = await _mediator.Send(getProjectDetailsRequest, CancellationToken.None).ConfigureAwait(false);

            SendProjectDetails(peer, getProjectDetailsResult);
        }

        private async Task HandleGetProjectDetailsByProjectNameAsync(NetPeer peer, NetDataReader reader)
        {
            var getProjectDetailsRequest = new GetProjectDetailsByProjectNameRequest(reader);
            var getProjectDetailsResult = await _mediator.Send(getProjectDetailsRequest, CancellationToken.None).ConfigureAwait(false);

            SendProjectDetails(peer, getProjectDetailsResult);
        }

        private void SendProjectDetails(NetPeer peer, Result<ProjectDetailDto> getProjectDetailsResult)
        {
            var writer = _writerPool.Get();
            writer.Reset();

            if (getProjectDetailsResult.IsSuccess)
            {
                var projectDetail = getProjectDetailsResult.Value;
                var createdAt = projectDetail.CreatedAt;
                var createdAtDateTime = createdAt.Ticks;
                var createdAtOffset = createdAt.Offset.Ticks;

                writer.Put(PacketIds.S2C.GetProjectDetailsOk);
                writer.Put(projectDetail.Id);
                writer.Put(projectDetail.Name);
                writer.Put(projectDetail.Description);
                writer.Put(projectDetail.CreatedBy);
                writer.Put(createdAtDateTime);
                writer.Put(createdAtOffset);
                writer.Put(projectDetail.Template);
                writer.Put(projectDetail.ClientVersion);
            }
            else
            {
                writer.Put(PacketIds.S2C.GetProjectDetailsError);
                writer.Put(getProjectDetailsResult.Error);
            }

            peer.Send(writer, DeliveryMethod.ReliableOrdered);
            _writerPool.Return(writer);
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

        public void Run()
        {
            _server.Start(_serverSettings.Port);
            _logger.Information("NetworkServer - Server running on port {@Port}", _server.LocalPort);

            while (_serverRunning)
            {
                _server.PollEvents();
                Thread.Sleep(15);
            }

            _server.Stop();
        }

        private void ConsoleOnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            if (e.Cancel)
            {
                _logger.Debug("NetworkServer - Shutting down...");
                _serverRunning = false;
            }
        }
    }
}
