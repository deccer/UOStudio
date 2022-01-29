using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Serilog;
using UOStudio.Common.Contracts;
using UOStudio.Server.Data;

namespace UOStudio.Server.Domain.GetUsers
{
    [UsedImplicitly]
    internal sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<IList<UserDto>>>
    {
        private readonly ILogger _logger;
        private readonly ILiteDbFactory _liteDbFactory;

        public GetUsersQueryHandler(
            ILogger logger,
            ILiteDbFactory liteDbFactory)
        {
            _logger = logger;
            _liteDbFactory = liteDbFactory;
        }

        public async Task<Result<IList<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            if (!request.User.IsInRole(Permission.CanListUser.Name))
            {
                return Result.Failure<IList<UserDto>>("No permission");
            }

            using var db = _liteDbFactory.CreateLiteDatabase();
            var users = (await db.GetCollection<User>(nameof(User))
                    .Include(u => u.Permissions)
                    .FindAllAsync())
                .Select(u => new UserDto { Id = u.Id, Name = u.Name })
                .ToList();

            return Result.Success<IList<UserDto>>(users);
        }
    }
}
