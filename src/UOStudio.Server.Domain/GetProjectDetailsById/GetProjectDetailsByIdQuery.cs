using System.Security.Principal;
using CSharpFunctionalExtensions;
using MediatR;
using UOStudio.Common.Contracts;

namespace UOStudio.Server.Domain.GetProjectDetailsById
{
    public class GetProjectDetailsByIdQuery : IRequest<Result<ProjectDetailDto>>
    {
        public GetProjectDetailsByIdQuery(IPrincipal user, int projectId)
        {
            ProjectId = projectId;
            User = user;
        }

        public int ProjectId { get; }

        public IPrincipal User { get; }
    }
}
