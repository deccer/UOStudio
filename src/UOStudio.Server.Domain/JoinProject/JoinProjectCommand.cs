using System;
using System.Security.Principal;
using CSharpFunctionalExtensions;
using MediatR;
using UOStudio.Common.Contracts;

namespace UOStudio.Server.Domain.JoinProject
{
    public class JoinProjectCommand : IRequest<Result<JoinProjectResult>>
    {
        public JoinProjectCommand(
            IPrincipal user,
            int projectId,
            JoinProjectRequest joinProjectRequest)
        {
            ProjectId = projectId;
            AtlasHash = joinProjectRequest.AtlasHash;
            User = user;
        }

        public int ProjectId { get; }

        public string AtlasHash { get; }

        public IPrincipal User { get; }
    }
}
