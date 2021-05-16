using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UOStudio.Common.Contracts;
using UOStudio.Server.Api.Services;

namespace UOStudio.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;

        public AuthController(
            IAuthenticationService authenticationService,
            IUserService userService)
        {
            _authenticationService = authenticationService;
            _userService = userService;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Login([FromBody] UserCredentials userCredentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var tokenResult = await _authenticationService.AuthenticateAsync(userCredentials);
            if (tokenResult.IsFailure)
            {
                return Unauthorized(tokenResult.Error);
            }

            Response.Headers.Add("X-Ticket", tokenResult.Value.ConnectionTicket);
            return Ok(tokenResult.Value.TokenPair);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("validateTicket/{connectionTicket}")]
        public async Task<IActionResult> ValidateConnectionTicket([FromRoute] string connectionTicket)
        {
            var connectionTicketDecoded = UrlDecodeBase64(connectionTicket);
            var isValidConnectionTicket = await _userService.ValidateConnectionTicketAsync(connectionTicketDecoded);
            return isValidConnectionTicket
                ? Ok()
                : Unauthorized();
        }

        [Authorize]
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshToken refreshToken)
        {
            var token = refreshToken.Token;
            var refreshTokenResult = await _authenticationService.RefreshTokenAsync(token);
            return refreshTokenResult.IsFailure
                ? BadRequest(refreshTokenResult.Error)
                : Ok(refreshTokenResult.Value);
        }

        public static string UrlDecodeBase64(string encodedBase64Input)
        {
            return encodedBase64Input
                .Replace('.', '+')
                .Replace('_', '/')
                .Replace('-', '=');
        }
    }
}
