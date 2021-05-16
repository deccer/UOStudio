using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UOStudio.Common.Contracts;
using UOStudio.Server.Data;

namespace UOStudio.Server.Api.Services
{
    public sealed class TokenService : ITokenService
    {
        private readonly IUserRepository _userRepository;
        private readonly RSA _rsa;
        private readonly byte[] _privateKey;
        private readonly string _issuer;
        private readonly string _audience;

        public TokenService(
            IUserRepository userRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _privateKey = Convert.FromBase64String(configuration["JwtToken:RsaPrivateKey"]);
            _issuer = configuration["JwtToken:Issuer"];
            _audience = configuration["JwtToken:Audience"];
            _rsa = RSA.Create();
        }

        public async Task<Result<TokenPair>> GetTokenAsync(string userName)
        {
            var userResult = await _userRepository.GetUserAsync(userName);
            return await CreateTokenAndUpdateRefreshTokenAsync(userResult);
        }

        public async Task<Result<TokenPair>> GetTokenFromRefreshTokenAsync(string refreshToken)
        {
            var userResult = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);
            return await CreateTokenAndUpdateRefreshTokenAsync(userResult);
        }

        private async Task<Result<TokenPair>> CreateTokenAndUpdateRefreshTokenAsync(Result<UserDto> userResult)
        {
            if (userResult.IsFailure)
            {
                return Result.Failure<TokenPair>(userResult.Error);
            }

            var createTokenPairResult = await CreateTokenPairAsync(userResult.Value);
            if (createTokenPairResult.IsFailure)
            {
                return Result.Failure<TokenPair>(createTokenPairResult.Error);
            }

            return await UpdateRefreshTokenAsync(userResult, createTokenPairResult);
        }

        private async Task<Result<TokenPair>> CreateTokenPairAsync(UserDto user)
        {
            var tokenDescriptorResult = await GetTokenDescriptorAsync(user);
            if (tokenDescriptorResult.IsFailure)
            {
                return Result.Failure<TokenPair>(tokenDescriptorResult.Error);
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptorResult.Value);

            return Result.Success(
                new TokenPair
                {
                    AccessToken = tokenHandler.WriteToken(securityToken),
                    RefreshToken = GenerateRefreshToken()
                }
            );
        }

        private async Task<Result<TokenPair>> UpdateRefreshTokenAsync(Result<UserDto> userResult, Result<TokenPair> createTokenPairResult)
        {
            var updateRefreshTokenResult = await _userRepository
                .UpdateRefreshTokenAsync(userResult.Value.Id, createTokenPairResult.Value.RefreshToken);
            return updateRefreshTokenResult.IsFailure
                ? Result.Failure<TokenPair>(updateRefreshTokenResult.Error)
                : createTokenPairResult.Value;
        }

        private static string GenerateRefreshToken()
        {
            using var rng = new RNGCryptoServiceProvider();
            var bytes = new byte[32];
            rng.GetNonZeroBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private async Task<Result<SecurityTokenDescriptor>> GetTokenDescriptorAsync(UserDto user)
        {
            const int expiringDays = 7;

            var certificateResult = await GetSigningCredentialsAsync();
            if (certificateResult.IsFailure)
            {
                return Result.Failure<SecurityTokenDescriptor>(certificateResult.Error);
            }

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(user.Claims),
                Expires = now.AddDays(expiringDays),
                Audience = _audience,
                Issuer = _issuer,
                IssuedAt = now,
                NotBefore = now,
                SigningCredentials = certificateResult.Value
            };

            return Result.Success(tokenDescriptor);
        }

        private async Task<Result<SigningCredentials>> GetSigningCredentialsAsync()
        {
            _rsa.ImportRSAPrivateKey(_privateKey, out _);
            var rsaSecurityKey = new RsaSecurityKey(_rsa);

            return new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);
        }
    }
}
