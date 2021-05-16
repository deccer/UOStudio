using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UOStudio.Common.Contracts;
using UOStudio.Server.Data;

namespace UOStudio.Server.Domain.GetUsers
{
    [UsedImplicitly]
    internal sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<IList<UserDto>>>
    {
        private readonly ILogger _logger;
        private readonly IDbContextFactory<UOStudioContext> _contextFactory;

        public GetUsersQueryHandler(
            ILogger logger,
            IDbContextFactory<UOStudioContext> contextFactory)
        {
            _logger = logger;
            _contextFactory = contextFactory;
        }

        public async Task<Result<IList<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            if (!request.User.IsInRole(Permission.CanListUser.Name))
            {
                return Result.Failure<IList<UserDto>>("No permission");
            }

            await using var db = _contextFactory.CreateDbContext();
            var users = await db.Users
                .AsNoTracking()
                .Include(u => u.Permissions)
                .Select(u => new UserDto { Id = u.Id, Name = u.Name })
                .ToListAsync(cancellationToken);

            return Result.Success<IList<UserDto>>(users);
        }
    }
}
