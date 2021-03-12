using System.Security.Principal;
using CSharpFunctionalExtensions;
using MediatR;

namespace UOStudio.Server.Domain.DeleteProjectTemplate
{
    public class DeleteProjectTemplateCommand : IRequest<Result>
    {
        public DeleteProjectTemplateCommand(IPrincipal user, int projectTemplateId)
        {
            User = user;
            ProjectTemplateId = projectTemplateId;
        }

        public IPrincipal User { get; }

        public int ProjectTemplateId { get; }
    }
}
