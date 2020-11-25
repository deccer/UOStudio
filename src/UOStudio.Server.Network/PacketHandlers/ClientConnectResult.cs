using System;
using System.Collections.Generic;
using UOStudio.Server.Data;

namespace UOStudio.Server.Network.PacketHandlers
{
    public class ClientConnectResult
    {
        public ClientConnectResult(Guid accountId, IList<Project> projects)
        {
            AccountId = accountId;
            Projects = projects;
        }

        public Guid AccountId { get; }

        public IList<Project> Projects { get; }
    }
}
