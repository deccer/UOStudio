using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UOStudio.Common.Contracts;
using UOStudio.Common.Core;
using UOStudio.Server.Api.Models;
using UOStudio.Server.Data;

namespace UOStudio.Server.Api.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger _logger;
        private readonly ITokenService _tokenService;
        private readonly IDbContextFactory<UOStudioContext> _contextFactory;
        private readonly IPasswordVerifier _passwordVerifier;

        public UserService(
            ILogger logger,
            ITokenService tokenService,
            IDbContextFactory<UOStudioContext> contextFactory,
            IPasswordVerifier passwordVerifier)
        {
            _logger = logger;
            _tokenService = tokenService;
            _contextFactory = contextFactory;
            _passwordVerifier = passwordVerifier;
        }

        public async Task<Result<Tokens>> LoginAsync(
            AuthenticationRequest authenticationRequest,
            CancellationToken cancellationToken)
        {
            await using var db = _contextFactory.CreateDbContext();
            var user = await db.Users
                .Include(u => u.Permissions)
                .FirstOrDefaultAsync(u => u.Name == authenticationRequest.UserName, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return user == null || !_passwordVerifier.Verify(authenticationRequest.Password, user.Password)
                ? Result.Failure<Tokens>("Invalid credentials")
                : Result.Success(await GenerateTokensAsync(db, user, cancellationToken));
        }

        public async Task<Result> RefreshAsync(int userId, string refreshToken, CancellationToken cancellationToken)
        {
            await using var db = _contextFactory.CreateDbContext();
            var user = await db.Users
                .Include(u => u.RefreshToken)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (user == null)
            {
                return Result.Failure<Task>("User not found");
            }

            if (string.IsNullOrEmpty(user.RefreshToken?.Value))
            {
                return Result.Failure<Task>("User never logged in");
            }

            return user.RefreshToken.Value != refreshToken
                ? Result.Failure<Task>("Incorrect refresh token")
                : Result.Success(await GenerateTokensAsync(db, user, cancellationToken));
        }

        private async Task<Tokens> GenerateTokensAsync(DbContext db, User user, CancellationToken cancellationToken)
        {
            var refreshToken = _tokenService.GenerateRefreshTokenAsync();
            user.RefreshToken = new RefreshToken { Value = refreshToken };

            try
            {
                db.Update(user);
                await db.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }

            var accessToken = await _tokenService.GenerateAccessToken(user, refreshToken);
            return new Tokens
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
