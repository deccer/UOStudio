using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UOStudio.Common.Contracts;
using UOStudio.Server.Common;
using UOStudio.Server.Domain.CreateProject;
using UOStudio.Server.Domain.GetProjectDetailsById;
using UOStudio.Server.Domain.GetProjectDetailsByName;
using UOStudio.Server.Domain.GetProjects;
using UOStudio.Server.Domain.JoinProject;

namespace UOStudio.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProjectController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Get()
        {
            var getProjectsCommand = new GetProjectsQuery(User);
            var getProjectsResult = await _mediator.Send(getProjectsCommand);

            return getProjectsResult.IsSuccess
                ? Ok(getProjectsResult.Value)
                : BadRequest(getProjectsResult.Error);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest createProjectRequest)
        {
            var createProjectCommand = new CreateProjectCommand(User, createProjectRequest);
            var createProjectResult = await _mediator.Send(createProjectCommand);

            return createProjectResult.IsSuccess
                ? CreatedAtAction(nameof(GetDetails), createProjectResult.Value)
                : BadRequest(createProjectResult.Error);
        }

        [HttpGet]
        [Route("{projectIdOrName}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public Task<IActionResult> GetDetails([FromRoute] string projectIdOrName)
        {
            return int.TryParse(projectIdOrName, out var projectId)
                ? GetDetailsById(projectId)
                : GetDetailsByName(projectIdOrName);
        }

        [HttpPost]
        [Route("{projectId:int}/join/")]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> JoinProject(int projectId, [FromBody] JoinProjectRequest joinProjectRequest)
        {
            var joinProjectCommand = new JoinProjectCommand(User, projectId, joinProjectRequest);
            var joinProjectResult = await _mediator.Send(joinProjectCommand);

            return joinProjectResult.IsSuccess
                ? joinProjectResult.Value.NeedsPreparation
                    ? AcceptedAtRoute(nameof(BackgroundTaskController.GetBackgroundTask), new { backgroundTaskId = joinProjectResult.Value.TaskId })
                    : Ok(joinProjectResult.Value.TaskId)
                : joinProjectResult.IsSuccess
                    ? Ok(joinProjectResult.Value)
                    : BadRequest(joinProjectResult.Error);
        }

        private async Task<IActionResult> GetDetailsById(int projectId)
        {
            var getProjectDetailsRequest = new GetProjectDetailsByIdQuery(User, projectId);
            var getProjectDetailsResult = await _mediator.Send(getProjectDetailsRequest);

            return getProjectDetailsResult.IsSuccess
                ? Ok(getProjectDetailsResult.Value)
                : BadRequest(getProjectDetailsResult.Error);
        }

        private async Task<IActionResult> GetDetailsByName(string projectName)
        {
            projectName = projectName.Replace("+", " ");
            var getProjectDetailsRequest = new GetProjectDetailsByNameQuery(User, projectName);
            var getProjectDetailsResult = await _mediator.Send(getProjectDetailsRequest);

            return getProjectDetailsResult.IsSuccess
                ? Ok(getProjectDetailsResult.Value)
                : BadRequest(getProjectDetailsResult.Error);
        }
    }
}
