using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UOStudio.Common.Contracts;
using UOStudio.Server.Api.Models;
using UOStudio.Server.Api.Services;
using UOStudio.Server.Data;
using UOStudio.Server.Domain.GetUsers;

namespace UOStudio.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly IMediator _mediator;

        public UserController(
            ILogger logger,
            IUserService userService,
            IMediator mediator)
        {
            _logger = logger;
            _userService = userService;
            _mediator = mediator;
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var getUsersQuery = new GetUsersQuery(User);
            var getUsersQueryResult = await _mediator.Send(getUsersQuery).ConfigureAwait(false);

            return getUsersQueryResult.IsSuccess
                ? Ok(getUsersQueryResult.Value)
                : Forbid(getUsersQueryResult.Error);
        }
    }
}
