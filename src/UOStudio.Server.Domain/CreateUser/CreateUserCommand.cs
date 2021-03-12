using System.Collections.Generic;
using System.Security.Principal;
using CSharpFunctionalExtensions;
using MediatR;

namespace UOStudio.Server.Domain.CreateUser
{
    public class CreateUserCommand : IRequest<Result<int>>
    {
        public CreateUserCommand(
            IPrincipal user,
            string userName,
            string password,
            IList<string> permissions
        )
        {
            User = user;
            UserName = userName;
            Password = password;
            Permissions = permissions;
        }

        public IPrincipal User { get; }

        public string UserName { get; }

        public string Password { get; }

        public IList<string> Permissions { get; }
    }
}
