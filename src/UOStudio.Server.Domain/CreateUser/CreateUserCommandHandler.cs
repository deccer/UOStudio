using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UOStudio.Common.Core;
using UOStudio.Server.Data;

namespace UOStudio.Server.Domain.CreateUser
{
    [UsedImplicitly]
    internal sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<int>>
    {
        private readonly ILogger _logger;
        private readonly IDbContextFactory<UOStudioContext> _contextFactory;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUserCommandHandler(
            ILogger logger,
            IDbContextFactory<UOStudioContext> contextFactory,
            IPasswordHasher passwordHasher)
        {
            _logger = logger.ForContext<CreateUserCommandHandler>();
            _contextFactory = contextFactory;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<int>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (!request.User.IsInRole(Permission.CanCreateUser.Name))
            {
                return Result.Failure<int>("No permission");
            }

            await using var db = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var user = await db.Users
                .AsQueryable()
                .Where(u => u.Name == request.UserName)
                .FirstOrDefaultAsync(cancellationToken);

            if (user != null)
            {
                return Result.Failure<int>($"User '{request.UserName}' already exists");
            }

            var permissions = Permission.AllPermissions.Where(permission => request.Permissions.Any(p => p == permission.Name));
            var userPassword = _passwordHasher.Hash(request.Password);
            user = new User
            {
                Name = request.UserName,
                Password = userPassword.HashedPassword,
                Nonce = userPassword.Salt,
                Permissions = new List<Permission>(permissions)
            };

            await db.Users.AddAsync(user, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            return Result.Success(user.Id);
        }
    }
}
