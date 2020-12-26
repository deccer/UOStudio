using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using UOStudio.Client.Core;

namespace UOStudio.Client.Network
{
    public interface INetworkClient
    {
        event Action<EndPoint, int> Connected;

        event Action Disconnected;

        event Action<Guid> LoginSuccessful;

        event Action<string> LoginFailed;

        event Action<string> JoinProjectSuccessful;

        event Action<string> JoinProjectFailed;

        event Action<double> DownloadProgress;

        bool IsConnected { get; }

        void Connect(Profile profile);

        void Disconnect();

        void JoinProject(Guid projectId);

        void LeaveProject(Guid projectId);

        void SendChatMessage(string message);

        void Update();

        Task<IReadOnlyCollection<Project>> GetProjectsAsync();

        Task<Result<Guid>> CreateProjectAsync(string projectName, string projectDescription, string projectClientVersion);

        Task<Result> DeleteProjectAsync(Guid projectId);
    }
}
