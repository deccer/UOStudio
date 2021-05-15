using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace UOStudio.Client.Services
{
    public sealed class TokenHandler : ITokenHandler
    {
        private readonly JwtSecurityTokenHandler _securityTokenHandler;

        public TokenHandler()
        {
            _securityTokenHandler = new JwtSecurityTokenHandler();
        }

        public IPrincipal GetUserFromAccessToken(string accessToken)
        {
            var token = _securityTokenHandler.ReadJwtToken(accessToken);
            return new ClaimsPrincipal(new ClaimsIdentity(token.Claims));
        }
    }
}
