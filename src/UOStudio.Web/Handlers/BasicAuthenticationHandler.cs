using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UOStudio.Core;
using UOStudio.Web.Contracts;
using UOStudio.Web.Services;

namespace UOStudio.Web.Handlers
{
    public sealed class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserService _userService;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserService userService)
            : base(options, logger, encoder, clock) =>
            _userService = userService;

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
            {
                return AuthenticateResult.NoResult();
            }

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            UserDto user;
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter!);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                var username = credentials[0];
                var password = credentials[1];
                user = await _userService.AuthenticateAsync(username, password);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            if (user == null)
            {
                return AuthenticateResult.Fail("Invalid Username or Password");
            }

            var principal = CreatePrincipalFromUser(user);

            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        private ClaimsPrincipal CreatePrincipalFromUser(UserDto user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name),
                new("User.IsActive", user.IsActive.ToString()),
                new("User.IsBlocked", user.IsBlocked.ToString())
            };

            if (user.Permissions.HasFlag(Permissions.Viewer))
            {
                claims.Add(new Claim(ClaimTypes.Role, nameof(Permissions.Viewer)));
            }

            if (user.Permissions.HasFlag(Permissions.Editor))
            {
                claims.Add(new Claim(ClaimTypes.Role, nameof(Permissions.Editor)));
            }

            if (user.Permissions.HasFlag(Permissions.Backup))
            {
                claims.Add(new Claim(ClaimTypes.Role, nameof(Permissions.Backup)));
            }
            if (user.Permissions.HasFlag(Permissions.Administrator))
            {
                claims.Add(new Claim(ClaimTypes.Role, nameof(Permissions.Administrator)));
            }

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            return new ClaimsPrincipal(identity);
        }
    }
}
