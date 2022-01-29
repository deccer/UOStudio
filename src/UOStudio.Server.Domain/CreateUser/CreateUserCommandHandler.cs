using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Serilog;
using UOStudio.Common.Core;
using UOStudio.Server.Data;

namespace UOStudio.Server.Domain.CreateUser
{
    [UsedImplicitly]
    internal sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<int>>
    {
        private readonly ILogger _logger;
        private readonly ILiteDbFactory _liteDbFactory;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUserCommandHandler(
            ILogger logger,
            ILiteDbFactory liteDbFactory,
            IPasswordHasher passwordHasher)
        {
            _logger = logger.ForContext<CreateUserCommandHandler>();
            _liteDbFactory = liteDbFactory;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<int>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (!request.User.IsInRole(Permission.CanCreateUser.Name))
            {
                return Result.Failure<int>("No permission");
            }

            using var db = _liteDbFactory.CreateLiteDatabase();
            var users = db.GetCollection<User>(nameof(User));
            var user = await users
                .FindOneAsync(u => u.Name == request.UserName);

            if (user != null)
            {
                return Result.Failure<int>($"User '{request.UserName}' already exists");
            }

            var permissions = Permission.AllPermissions
                .Where(permission => request.Permissions.Any(p => p == permission.Name));
            var userPassword = _passwordHasher.Hash(request.Password);
            user = new User
            {
                Name = request.UserName,
                Password = userPassword.HashedPassword,
                Nonce = userPassword.Salt,
                Permissions = new List<Permission>(permissions)
            };

            await users.InsertAsync(user);
            await db.CommitAsync();

            return Result.Success(user.Id);
        }
    }
}
