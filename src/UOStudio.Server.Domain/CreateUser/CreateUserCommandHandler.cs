using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UOStudio.Server.Data;

namespace UOStudio.Server.Domain.CreateUser
{
    [UsedImplicitly]
    internal sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<int>>
    {
        private readonly ILogger _logger;
        private readonly IDbContextFactory<UOStudioContext> _contextFactory;

        public CreateUserCommandHandler(ILogger logger, IDbContextFactory<UOStudioContext> contextFactory)
        {
            _logger = logger.ForContext<CreateUserCommandHandler>();
            _contextFactory = contextFactory;
        }

        public async Task<Result<int>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (!request.User.IsInRole(Permission.CanCreateUser.Name))
            {
                return Result.Failure<int>("No permission");
            }

            await using var db = _contextFactory.CreateDbContext();
            var user = await db.Users
                .AsQueryable()
                .FirstOrDefaultAsync(u => u.Name == request.UserName, cancellationToken);
            if (user != null)
            {
                return Result.Failure<int>($"User '{request.UserName}' already exists");
            }

            var permissions = Permission.AllPermissions.Where(permission => request.Permissions.Any(p => p == permission.Name));
            user = new User
            {
                Name = request.UserName,
                Password = request.Password,
                Permissions = new List<Permission>(permissions)
            };

            await db.Users.AddAsync(user, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            return Result.Success(user.Id);
        }
    }
}
