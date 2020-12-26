using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using LiteNetLib;
using LiteNetLib.Utils;
using Serilog;
using UOStudio.Client.Core;
using UOStudio.Client.Core.Settings;
using UOStudio.Core.Extensions;
using UOStudio.Core.Network;
using UOStudio.Shared.Network;

namespace UOStudio.Client.Network
{
    public class NetworkClient : INetworkClient
    {
        private readonly ILogger _logger;
        private readonly IAppSettingsProvider _appSettingsProvider;
        private readonly HttpClient _httpClient;

        private readonly NetManager _client;
        private NetPeer _peerConnection;
        private readonly NetDataWriter _writer;

        private Profile _profile;
        private Guid _currentUserId;
        private Project _currentProject;
        private readonly string _projectsPath;

        public event Action<EndPoint, int> Connected;

        public event Action Disconnected;

        public event Action<Guid> LoginSuccessful;

        public event Action<string> LoginFailed;

        public event Action<string> JoinProjectSuccessful;

        public event Action<string> JoinProjectFailed;

        public event Action<double> DownloadProgress;

        public bool IsConnected { get; private set; }

        public NetworkClient(ILogger logger, IAppSettingsProvider appSettingsProvider, HttpClient httpClient)
        {
            _logger = logger;
            _appSettingsProvider = appSettingsProvider;
            _httpClient = httpClient;

            var listener = new EventBasedNetListener();
            _client = new NetManager(listener);

            listener.NetworkReceiveEvent += async (netPeer, netReader, deliveryMethod) => await ListenerOnNetworkReceiveEvent(netPeer, netReader, deliveryMethod);
            listener.PeerConnectedEvent += ListenerOnConnectedEvent;
            listener.PeerDisconnectedEvent += ListenerOnDisconnectedEvent;
            listener.NetworkErrorEvent += ListenerOnNetworkErrorEvent;

            _writer = new NetDataWriter();
            _projectsPath = _appSettingsProvider.AppSettings.General.ProjectsPath;
        }

        public void Update()
        {
            _client.PollEvents();
        }

        public void Connect(Profile profile)
        {
            _profile = profile;
            _logger.Debug($"NetworkClient - Connecting to {_profile.ServerName}:{_profile.ServerPort}...");
            if (IsConnected)
            {
                _client.Stop();
            }
            _client.Start();

            _peerConnection = _client.Connect(_profile.ServerName, _profile.ServerPort, "UOStudio");
        }

        public void Disconnect()
        {
            if (!IsConnected)
            {
                return;
            }

            _logger.Debug("Disconnecting...");
            _client.DisconnectPeer(_peerConnection);
            _logger.Debug("Disconnecting...Done");
        }

        public void JoinProject(Guid projectId)
        {
            var projectClientHash = new byte[16];
            for (var i = 0; i < projectClientHash.Length; ++i)
            {
                projectClientHash[i] = 0;
            }

            var projectPath = Path.Combine(_projectsPath, projectId.ToString("N"));
            var projectPathInfo = new DirectoryInfo(projectPath);
            if (projectPathInfo.Exists)
            {
                using var hashAlgorithm = MD5.Create();
                projectClientHash = hashAlgorithm
                    .ComputeHashAsync(projectPathInfo.EnumerateFiles("*.mul", SearchOption.TopDirectoryOnly), false)
                    .GetAwaiter()
                    .GetResult();
            }

            if (!IsConnected)
            {
                return;
            }

            _logger.Debug($"Join Project: {projectId:N}");

            _writer.Reset();
            _writer.Put((int)RequestIds.C2S.JoinProject);
            _writer.Put(_currentUserId.ToString("N"));
            _writer.Put(projectId.ToString("N"));
            _writer.Put(projectClientHash);
            _peerConnection.Send(_writer, DeliveryMethod.ReliableOrdered);
        }

        public void LeaveProject(Guid projectId)
        {
            if (!IsConnected)
            {
                return;
            }

            _logger.Debug($"Leave Project: {projectId:N}");

            _writer.Reset();
            _writer.Put((int)RequestIds.C2S.JoinProject);
            _writer.Put(_currentUserId.ToString("N"));
            _writer.Put(projectId.ToString("N"));
            _peerConnection.Send(_writer, DeliveryMethod.ReliableOrdered);
        }

        public void SendChatMessage(string message)
        {
            if (!IsConnected)
            {
                return;
            }

            _logger.Debug($"Chat Message: {message}");

            _writer.Reset();
            _writer.Put((int)RequestIds.C2S.ChatMessage);
            _writer.Put(_currentUserId.ToString("N"));
            _writer.Put(message);
            _peerConnection.Send(_writer, DeliveryMethod.ReliableOrdered);
        }

        public async Task<IReadOnlyCollection<Project>> GetProjectsAsync()
        {
            var response = await _httpClient.GetAsync($"{_appSettingsProvider.AppSettings.General.UOStudioBaseUrl}api/project");
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsAsync<Project[]>()
                : null;
        }

        public async Task<Result<Guid>> CreateProjectAsync(string projectName, string projectDescription, string projectClientVersion)
        {
            var projectModel = new
            {
                Name = projectName,
                Description = projectDescription,
                ClientVersion = projectClientVersion
            };

            var response = await _httpClient.PostAsJsonAsync($"{_appSettingsProvider.AppSettings.General.UOStudioBaseUrl}api/project", projectModel);
            if (response.IsSuccessStatusCode)
            {
                var projectId = await response.Content.ReadAsAsync<Guid>();
                return Result.Success(projectId);
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return Result.Failure<Guid>(errorMessage);
        }

        public async Task<Result> DeleteProjectAsync(Guid projectId)
        {
            var response = await _httpClient.DeleteAsync($"{_appSettingsProvider.AppSettings.General.UOStudioBaseUrl}api/project/{projectId:N}");
            return response.IsSuccessStatusCode
                ? Result.Success()
                : Result.Failure(await response.Content.ReadAsStringAsync());
        }

        private async Task ListenerOnNetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var requestId = (RequestIds.S2C)reader.GetInt();
            _logger.Debug($"RequestId: {requestId}");
            switch (requestId)
            {
                case RequestIds.S2C.ConnectSuccess:
                    await HandleConnectSuccess(reader);
                    break;
                case RequestIds.S2C.ConnectFailed:
                    await HandleConnectFailed(peer, reader);
                    break;
                case RequestIds.S2C.JoinProjectSuccess:
                    await HandleJoinProjectSuccess(peer, reader);
                    break;
                case RequestIds.S2C.JoinProjectFailed:
                    await HandleJoinProjectFailed(peer, reader);
                    break;
                case RequestIds.S2C.SystemMessage:
                    await HandleSystemMessage(peer, reader);
                    break;
                case RequestIds.S2C.ChatMessage:
                    await HandleChatMessage(peer, reader);
                    break;
                case RequestIds.S2C.RefreshUsers:
                    await HandleRefreshUsers(peer, reader);
                    break;
                case RequestIds.S2C.LeaveProjectSuccess:
                    break;
                case RequestIds.S2C.LeaveProjectFailed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            reader.Recycle();
        }

        private void ListenerOnDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            _logger.Debug($"NetworkClient - Disconnected: {peer.EndPoint}");
            Disconnected?.Invoke();
            IsConnected = false;
        }

        private void ListenerOnConnectedEvent(NetPeer peer)
        {
            _logger.Debug($"NetworkClient - Connected to {peer.EndPoint}");
            Connected?.Invoke(peer.EndPoint, peer.Id);
            IsConnected = true;

            _writer.Reset();
            _writer.Put((int)RequestIds.C2S.Connect);
            _writer.Put(_profile.AccountName);
            _writer.Put(_profile.AccountPassword);
            peer.Send(_writer, DeliveryMethod.ReliableOrdered);
        }

        private void ListenerOnNetworkErrorEvent(IPEndPoint endpoint, SocketError socketError)
        {
            _logger.Error($"Network Error {endpoint} - {socketError}");
        }

        private Task HandleRefreshUsers(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private Task HandleChatMessage(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private Task HandleSystemMessage(NetPeer peer, NetPacketReader reader)
        {
            throw new NotImplementedException();
        }

        private Task HandleConnectSuccess(NetDataReader reader)
        {
            var userId = Guid.Parse(reader.GetString());
            _currentUserId = userId;

            var loginSuccessful = LoginSuccessful;
            loginSuccessful?.Invoke(_currentUserId);

            return Task.CompletedTask;
        }

        private Task HandleConnectFailed(NetPeer peer, NetDataReader reader)
        {
            _currentUserId = Guid.Empty;
            _currentProject = null;

            var loginFailed = LoginFailed;
            loginFailed?.Invoke(reader.GetString());

            return Task.CompletedTask;
        }

        private void HttpClientProgressChanged(
            long? totalDownloadSize,
            long currentSize,
            double? percentage,
            Guid projectId,
            string destinationFilePath)
        {
            var downloadProgress = DownloadProgress;
            downloadProgress?.Invoke(percentage.Value);
        }

        private async Task HandleJoinProjectSuccess(NetPeer peer, NetDataReader reader)
        {
            var projectId = Guid.ParseExact(reader.GetString(), "N");
            var joinResult = (JoinResult)reader.GetInt();

            if (joinResult == JoinResult.ClientRequiresUpdate)
            {
                var downloadUrl = $"{_appSettingsProvider.AppSettings.General.UOStudioBaseUrl}api/file/{projectId:N}";
                var tempFileName = Path.GetTempFileName();
                var httpClientProgressWrapper = new HttpClientProgressWrapper(_httpClient, downloadUrl, tempFileName, projectId);
                httpClientProgressWrapper.ProgressChanged += HttpClientProgressChanged;
                httpClientProgressWrapper.ProgressCompleted += HttpClientProgressWrapperOnProgressCompleted;
                await httpClientProgressWrapper.StartDownload().ConfigureAwait(false);

                /*
                if (response.IsSuccessStatusCode)
                {

                }
                else
                {
                    var joinProjectFailed = JoinProjectFailed;
                    joinProjectFailed?.Invoke("");
                }
                */
            }
            else
            {
                var projectPath = Path.Combine(_projectsPath, projectId.ToString("N"));
                Directory.CreateDirectory(projectPath);

                var joinProjectSuccessful = JoinProjectSuccessful;
                joinProjectSuccessful?.Invoke(projectPath);
            }
        }

        private void HttpClientProgressWrapperOnProgressCompleted(Guid projectId, string destinationFilePath)
        {
            var projectPath = Path.Combine(_projectsPath, projectId.ToString("N"));
            Directory.CreateDirectory(projectPath);

            ZipFile.ExtractToDirectory(destinationFilePath, projectPath);
            File.Delete(destinationFilePath);

            var joinProjectSuccessful = JoinProjectSuccessful;
            joinProjectSuccessful?.Invoke(projectPath);
        }

        private Task HandleJoinProjectFailed(NetPeer peer, NetPacketReader reader)
        {
            return Task.CompletedTask;
        }
    }
}
