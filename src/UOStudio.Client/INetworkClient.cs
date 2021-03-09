using System;
using System.Collections.Generic;
using LiteNetLib;
using UOStudio.Common.Contracts;

namespace UOStudio.Client
{
    public interface INetworkClient
    {
        event Action<NetPeer> Connected;

        event Action Disconnected;

        event Action<int, IReadOnlyCollection<ProjectDto>> LoginSucceeded;

        event Action<string> LoginFailed;

        event Action<ProjectDetailDto> GetProjectDetailsSucceeded;

        event Action<string> GetProjectDetailsFailed;

        public void Connect(Profile profile);

        public void Disconnect();

        public void Update();

        void GetProjectDetailsByProjectId(int projectId);

        void GetProjectDetailsByProjectName(string projectName);
    }
}
