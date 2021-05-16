using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UOStudio.Common.Contracts;
using UOStudio.Common.Core;
using UOStudio.Server.Data;

namespace UOStudio.Server.Api.Services
{
    public sealed class UserService : IUserService
    {
        private readonly ILogger _logger;
        private readonly IDbContextFactory<UOStudioContext> _contextFactory;
        private readonly IPasswordVerifier _passwordVerifier;

        public UserService(
            ILogger logger,
            IDbContextFactory<UOStudioContext> contextFactory,
            IPasswordVerifier passwordVerifier)
        {
            _logger = logger;
            _contextFactory = contextFactory;
            _passwordVerifier = passwordVerifier;
        }

        public async Task<Result> ValidateCredentialsAsync(UserCredentials userCredentials, CancellationToken cancellationToken = default)
        {
            await using var db = _contextFactory.CreateDbContext();

            var user = await db.Users.FirstOrDefaultAsync(u => u.Name == userCredentials.UserName, cancellationToken);
            var isValid = user != null && AreCredentialsValid(userCredentials, user);
            return !isValid
                ? Result.Failure("Invalid credentials")
                : Result.Success();
        }

        public async Task<Result> UpdateConnectionTicketAsync(string userName, string connectionTicket, CancellationToken cancellationToken = default)
        {
            await using var db = _contextFactory.CreateDbContext();

            var user = await db.Users.FirstOrDefaultAsync(u => u.Name == userName, cancellationToken);
            if (user == null)
            {
                return Result.Failure("User does not exist");
            }

            user.ConnectionTicket = connectionTicket;
            await db.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<bool> ValidateConnectionTicketAsync(string connectionTicket)
        {
            await using var db = _contextFactory.CreateDbContext();

            var userForConnectionTicket = await db.Users.SingleOrDefaultAsync(u => u.ConnectionTicket == connectionTicket);
            return userForConnectionTicket != null;
        }

        private bool AreCredentialsValid(UserCredentials userCredentials, User user)
            => user.Name == userCredentials.UserName && _passwordVerifier.Verify(userCredentials.Password, user.Password);
    }
}
