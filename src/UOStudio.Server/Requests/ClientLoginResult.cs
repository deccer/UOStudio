using System.Collections.Generic;
using UOStudio.Common.Contracts;

namespace UOStudio.Server.Requests
{
    public readonly struct ClientLoginResult
    {
        public ClientLoginResult(int userId, IList<ProjectDto> projects)
        {
            UserId = userId;
            Projects = projects;
        }

        public int UserId { get; }

        public IList<ProjectDto> Projects { get; }
    }
}
