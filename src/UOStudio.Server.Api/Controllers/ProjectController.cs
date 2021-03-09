using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using UOStudio.Common.Contracts;
using UOStudio.Server.Domain.CreateProject;
using UOStudio.Server.Domain.GetProjectDetailsById;
using UOStudio.Server.Domain.GetProjectDetailsByName;
using UOStudio.Server.Domain.GetProjects;

namespace UOStudio.Server.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public ProjectController(
            ILogger logger,
            IMediator mediator)
        {
            _logger = logger.ForContext<ProjectController>();
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var getProjectsCommand = new GetProjectsQuery(User);
            var getProjectsResult = await _mediator.Send(getProjectsCommand);

            return getProjectsResult.IsSuccess
                ? Ok(getProjectsResult.Value)
                : BadRequest(getProjectsResult.Error);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest createProjectRequest)
        {
            var createProjectCommand = new CreateProjectCommand(User, createProjectRequest);
            var createProjectResult = await _mediator.Send(createProjectCommand);

            return createProjectResult.IsSuccess
                ? CreatedAtAction(nameof(GetDetailsById), createProjectResult.Value)
                : BadRequest(createProjectResult.Error);
        }

        [HttpGet]
        [Route("details/id/{projectId:int}")]
        public async Task<IActionResult> GetDetailsById([FromRoute] int projectId)
        {
            var getProjectDetailsRequest = new GetProjectDetailsByIdQuery(projectId, User);
            var getProjectDetailsResult = await _mediator.Send(getProjectDetailsRequest);

            return getProjectDetailsResult.IsSuccess
                ? Ok(getProjectDetailsResult.Value)
                : BadRequest(getProjectDetailsResult.Error);
        }

        [HttpGet]
        [Route("details/name/{projectName:alpha}")]
        public async Task<IActionResult> GetDetailsByName([FromRoute] string projectName)
        {
            var getProjectDetailsRequest = new GetProjectDetailsByNameQuery(projectName, User);
            var getProjectDetailsResult = await _mediator.Send(getProjectDetailsRequest);

            return getProjectDetailsResult.IsSuccess
                ? Ok(getProjectDetailsResult.Value)
                : BadRequest(getProjectDetailsResult.Error);
        }
    }
}
