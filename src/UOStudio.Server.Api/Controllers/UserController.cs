using System.Security.Principal;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using UOStudio.Common.Contracts;
using UOStudio.Server.Domain.CreateUser;
using UOStudio.Server.Domain.GetUserById;
using UOStudio.Server.Domain.GetUsers;

namespace UOStudio.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public UserController(ILogger logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var getUsersQuery = new GetUsersQuery(User);
            var getUsersQueryResult = await _mediator.Send(getUsersQuery).ConfigureAwait(false);

            return getUsersQueryResult.IsSuccess
                ? Ok(getUsersQueryResult.Value)
                : Forbid(getUsersQueryResult.Error);
        }

        [HttpGet]
        [Route("{userId:int}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var getUserByIdQuery = new GetUserByIdQuery(User, userId);
            var getUserByIdResult = await _mediator.Send(getUserByIdQuery).ConfigureAwait(false);

            return getUserByIdResult.IsSuccess
                ? Ok(getUserByIdResult.Value)
                : BadRequest(getUserByIdResult.Error);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateUserRequest createUserRequest)
        {
            var createUserCommand = ToCommand(User, createUserRequest);
            var createUserResult = await _mediator.Send(createUserCommand).ConfigureAwait(false);

            return createUserResult.IsSuccess
                ? CreatedAtAction(nameof(GetUserById), createUserResult.Value)
                : BadRequest(createUserResult.Error);
        }

        private static CreateUserCommand ToCommand(IPrincipal principal, CreateUserRequest createUserRequest)
            => new(principal, createUserRequest.Name, createUserRequest.Password, createUserRequest.Permissions);
    }
}
