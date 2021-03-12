using System.Security.Principal;
using CSharpFunctionalExtensions;
using MediatR;
using UOStudio.Common.Contracts;

namespace UOStudio.Server.Domain.GetUserById
{
    public class GetUserByIdQuery : IRequest<Result<UserDto>>
    {
        public GetUserByIdQuery(IPrincipal user, int userId)
        {
            User = user;
            UserId = userId;
        }

        public IPrincipal User { get; }

        public int UserId { get; }
    }
}
