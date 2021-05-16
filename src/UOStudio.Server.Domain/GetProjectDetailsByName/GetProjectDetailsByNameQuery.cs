using System.Security.Principal;
using CSharpFunctionalExtensions;
using MediatR;
using UOStudio.Common.Contracts;

namespace UOStudio.Server.Domain.GetProjectDetailsByName
{
    public class GetProjectDetailsByNameQuery : IRequest<Result<ProjectDetailDto>>
    {
        public GetProjectDetailsByNameQuery(IPrincipal user, string projectName)
        {
            ProjectName = projectName;
            User = user;
        }

        public string ProjectName { get; }

        public IPrincipal User { get; }
    }
}
