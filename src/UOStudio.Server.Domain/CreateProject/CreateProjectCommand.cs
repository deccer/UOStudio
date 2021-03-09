using System.Security.Principal;
using CSharpFunctionalExtensions;
using MediatR;
using UOStudio.Common.Contracts;

namespace UOStudio.Server.Domain.CreateProject
{
    public class CreateProjectCommand : IRequest<Result<int>>
    {
        public CreateProjectCommand(
            IPrincipal user,
            CreateProjectRequest createProjectRequest)
        {
            User = user;
            ProjectName = createProjectRequest.Name;
            ProjectDescription = createProjectRequest.Description;
            TemplateId = createProjectRequest.TemplateId;
        }

        public IPrincipal User { get; }

        public string ProjectName { get; }

        public string ProjectDescription { get; }

        public int TemplateId { get; }
    }
}
