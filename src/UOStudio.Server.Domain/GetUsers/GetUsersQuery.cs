using System.Collections.Generic;
using System.Security.Principal;
using CSharpFunctionalExtensions;
using MediatR;
using UOStudio.Common.Contracts;

namespace UOStudio.Server.Domain.GetUsers
{
    public class GetUsersQuery : IRequest<Result<IList<UserDto>>>
    {
        public GetUsersQuery(IPrincipal user)
        {
            User = user;
        }

        public IPrincipal User { get; }
    }
}
