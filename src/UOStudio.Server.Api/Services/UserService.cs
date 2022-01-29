using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Serilog;
using UOStudio.Common.Contracts;
using UOStudio.Common.Core;
using UOStudio.Server.Data;

namespace UOStudio.Server.Api.Services
{
    public sealed class UserService : IUserService
    {
        private readonly ILogger _logger;
        private readonly ILiteDbFactory _liteDbFactory;
        private readonly IPasswordVerifier _passwordVerifier;

        public UserService(
            ILogger logger,
            ILiteDbFactory liteDbFactory,
            IPasswordVerifier passwordVerifier)
        {
            _logger = logger;
            _liteDbFactory = liteDbFactory;
            _passwordVerifier = passwordVerifier;
        }

        public async Task<Result> ValidateCredentialsAsync(UserCredentials userCredentials, CancellationToken cancellationToken = default)
        {
            using var db = _liteDbFactory.CreateLiteDatabase();
            var users = db.GetCollection<User>(nameof(User));

            var user = await users
                .FindOneAsync(u => u.Name == userCredentials.UserName);

            var isValidUser = user != null && AreCredentialsValid(user, userCredentials);
            return !isValidUser
                ? Result.Failure("Invalid credentials")
                : Result.Success();
        }

        public async Task<Result> UpdateConnectionTicketAsync(string userName, string connectionTicket, CancellationToken cancellationToken = default)
        {
            using var db = _liteDbFactory.CreateLiteDatabase();

            var users = db.GetCollection<User>(nameof(User));
            var user = await users
                .FindOneAsync(u => u.Name == userName);

            if (user == null)
            {
                return Result.Failure("User does not exist");
            }

            user.ConnectionTicket = connectionTicket;
            await users.UpdateAsync(user);
            await db.CommitAsync();

            return Result.Success();
        }

        public async Task<bool> ValidateConnectionTicketAsync(string connectionTicket)
        {
            using var db = _liteDbFactory.CreateLiteDatabase();

            var users = db.GetCollection<User>(nameof(User));
            var userForConnectionTicket = await users
                .FindOneAsync(u => u.ConnectionTicket == connectionTicket);

            return userForConnectionTicket != null;
        }

        private bool AreCredentialsValid(User user, UserCredentials userCredentials)
        {
            return user.Name == userCredentials.UserName && _passwordVerifier.Verify(userCredentials.Password, user.Nonce, user.Password);
        }
    }
}
