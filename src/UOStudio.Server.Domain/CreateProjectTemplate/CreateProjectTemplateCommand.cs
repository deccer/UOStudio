using System.Security.Principal;
using CSharpFunctionalExtensions;
using MediatR;

namespace UOStudio.Server.Domain.CreateProjectTemplate
{
    public class CreateProjectTemplateCommand : IRequest<Result<int>>
    {
        public CreateProjectTemplateCommand(
            IPrincipal user,
            string name,
            string clientVersion,
            string location
        )
        {
            Name = name;
            ClientVersion = clientVersion;
            Location = location;
        }

        public IPrincipal User { get; }

        public string Name { get; }

        public string ClientVersion { get; }

        public string Location { get; }
    }
}
