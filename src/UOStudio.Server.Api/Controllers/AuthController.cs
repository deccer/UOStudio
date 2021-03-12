using System.Linq;
using System.Security.Claims;
using System.Threading;
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
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("accessToken", Name = nameof(Login))]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest authenticationRequest)
        {
            var loginResult = await _userService.LoginAsync(authenticationRequest, CancellationToken.None);

            return loginResult.IsSuccess
                ? Ok(loginResult.Value)
                : BadRequest(loginResult.Error);
        }

        [Authorize(AuthenticationSchemes = "Refresh")]
        [HttpPut("accessToken", Name = nameof(RefreshToken))]
        public async Task<IActionResult> RefreshToken()
        {
            if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value, out var userId))
            {
                return BadRequest();
            }

            var refreshToken = User.Claims.FirstOrDefault(c => c.Type == "RefreshToken")?.Value;
            var refreshTokenResult = await _userService.RefreshAsync(userId, refreshToken, CancellationToken.None);

            return refreshTokenResult.IsFailure
                ? BadRequest(refreshTokenResult.Error)
                : Ok();
        }
    }
}
