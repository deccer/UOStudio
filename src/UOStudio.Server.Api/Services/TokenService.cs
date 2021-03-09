using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using UOStudio.Server.Data;

namespace UOStudio.Server.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly ILogger _logger;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _tokenExpiry;

        public TokenService(
            ILogger logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _secretKey = configuration["JwtToken:SecretKey"];
            _issuer = configuration["JwtToken:Issuer"];
            _audience = configuration["JwtToken:Audience"];
            _tokenExpiry = int.TryParse(configuration["JwtToken:TokenExpiry"], out var expiry)
                ? expiry
                : 60;
        }

        public async Task<string> GenerateAccessToken(User user, string refreshToken)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var issuer = _issuer;
            var audience = _audience;
            var expires = DateTime.Now.AddMinutes(_tokenExpiry);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new("RefreshToken", refreshToken)
            };
            if (user.Permissions != null)
            {
                var permissions = user.Permissions?
                    .Select(permission => new Claim(ClaimTypes.Role, permission.Name))
                    .ToArray();
                claims.AddRange(permissions);
            }

            var token = new JwtSecurityToken(issuer,
                audience,
                claims,
                expires: expires,
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        public string GenerateRefreshTokenAsync()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }
    }
}
