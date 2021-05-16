using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using UOStudio.Common.Contracts;

namespace UOStudio.Server.Api.Services
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthenticationService(
            IUserService userService,
            ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        public async Task<Result<TokenTriplet>> AuthenticateAsync(UserCredentials userCredentials)
        {
            var validationResult = await _userService.ValidateCredentialsAsync(userCredentials);
            if (validationResult.IsFailure)
            {
                return Result.Failure<TokenTriplet>(validationResult.Error);
            }

            var securityTokenResult = await _tokenService.GetTokenAsync(userCredentials.UserName);
            if (validationResult.IsFailure)
            {
                return Result.Failure<TokenTriplet>(securityTokenResult.Error);
            }

            var connectionTicket = GenerateConnectionTicket();
            var updateConnectionTicketResult = await _userService.UpdateConnectionTicketAsync(userCredentials.UserName, connectionTicket);
            if (updateConnectionTicketResult.IsFailure)
            {
                return Result.Failure<TokenTriplet>(updateConnectionTicketResult.Error);
            }

            var tokenTriplet = new TokenTriplet
            {
                TokenPair = securityTokenResult.Value,
                ConnectionTicket = connectionTicket
            };

            return Result.Success(tokenTriplet);
        }

        public async Task<Result<TokenPair>> RefreshTokenAsync(string refreshToken)
        {
            var newTokenResult = await _tokenService.GetTokenFromRefreshTokenAsync(refreshToken);
            return newTokenResult.IsFailure
                ? Result.Failure<TokenPair>(newTokenResult.Error)
                : newTokenResult.Value;
        }

        private string GenerateConnectionTicket()
        {
            Span<byte> connectionTicket = stackalloc byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetNonZeroBytes(connectionTicket);

            return Convert.ToBase64String(connectionTicket);
        }
    }
}
