using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace UOStudio.Client.Services
{
    public sealed class TokenService : ITokenService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _tokenEndpoint;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public TokenService(
            IConfiguration configuration,
            HttpClient httpClient,
            IMemoryCache memoryCache,
            IContext context)
        {
            _tokenEndpoint = configuration["Api:AuthenticationEndpoint"];
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _context = context;
            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = configuration["Api:AuthenticationIssuer"],
                ValidAudience = configuration["Api:AuthenticationAudience"],
                ValidateAudience = true,
                ValidateIssuer = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("UOStudioUOStudioUOStudioUOStudio"))
            };
        }

        public async Task<Result<string>> RetrieveTokenAsync(string userName, string password)
        {
            if (!_memoryCache.TryGetValue("Token", out string token))
            {
                var authenticationRequest = new
                {
                    UserName = userName,
                    Password = password
                };

                var response = await _httpClient.PostAsJsonAsync($"{_tokenEndpoint}/api/auth/accessToken/", authenticationRequest);

                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                var tokenHandler = new JwtSecurityTokenHandler();

                SecurityToken accessToken;
                try
                {
                    var principal = tokenHandler.ValidateToken(tokenResponse!.AccessToken, _tokenValidationParameters, out accessToken);
                    if (principal != null)
                    {
                        _context.User = principal;
                    }
                }
                catch (Exception exception)
                {
                    return Result.Failure<string>(exception.Message);
                }

                _memoryCache.Set("Token", tokenResponse.AccessToken, accessToken.ValidTo.Subtract(TimeSpan.FromSeconds(30)));
                return tokenResponse.AccessToken;
            }

            return Result.Success(token);
        }
    }
}
