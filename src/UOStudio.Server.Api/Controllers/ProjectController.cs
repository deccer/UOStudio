using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IMediator _mediator;

        public ProjectController(
            IMediator mediator)
        {
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
                ? CreatedAtAction(nameof(GetDetails), createProjectResult.Value)
                : BadRequest(createProjectResult.Error);
        }

        [HttpGet]
        [Route("{projectIdOrName}")]
        public Task<IActionResult> GetDetails([FromRoute] string projectIdOrName)
        {
            return int.TryParse(projectIdOrName, out var projectId)
                ? GetDetailsById(projectId)
                : GetDetailsByName(projectIdOrName);
        }

        private async Task<IActionResult> GetDetailsById(int projectId)
        {
            var getProjectDetailsRequest = new GetProjectDetailsByIdQuery(projectId, User);
            var getProjectDetailsResult = await _mediator.Send(getProjectDetailsRequest);

            return getProjectDetailsResult.IsSuccess
                ? Ok(getProjectDetailsResult.Value)
                : BadRequest(getProjectDetailsResult.Error);
        }

        private async Task<IActionResult> GetDetailsByName(string projectName)
        {
            projectName = projectName.Replace("+", " ");
            var getProjectDetailsRequest = new GetProjectDetailsByNameQuery(projectName, User);
            var getProjectDetailsResult = await _mediator.Send(getProjectDetailsRequest);

            return getProjectDetailsResult.IsSuccess
                ? Ok(getProjectDetailsResult.Value)
                : BadRequest(getProjectDetailsResult.Error);
        }
    }
}
