using System.Collections.Generic;
using System.Security.Principal;
using CSharpFunctionalExtensions;
using MediatR;
using UOStudio.Common.Contracts;

namespace UOStudio.Server.Domain.GetProjects
{
    public class GetProjectsQuery : IRequest<Result<IList<ProjectDto>>>
    {
        public GetProjectsQuery(IPrincipal user)
        {
            User = user;
        }

        public IPrincipal User { get; }
    }
}
