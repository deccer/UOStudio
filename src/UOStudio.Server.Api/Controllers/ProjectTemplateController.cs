using System.Security.Principal;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using UOStudio.Common.Contracts;
using UOStudio.Server.Domain.CreateProjectTemplate;
using UOStudio.Server.Domain.DeleteProjectTemplate;
using UOStudio.Server.Domain.GetProjectTemplates;

namespace UOStudio.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProjectTemplateController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public ProjectTemplateController(
            ILogger logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProjectTemplate(CreateProjectTemplateRequest createProjectTemplateRequest)
        {
            var createProjectTemplateCommand = ToCommand(User, createProjectTemplateRequest);
            var createProjectTemplateResult = await _mediator.Send(createProjectTemplateCommand);

            return createProjectTemplateResult.IsSuccess
                ? Ok(createProjectTemplateResult.Value)
                : BadRequest(createProjectTemplateResult.Error);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var getProjectTemplatesQuery = new GetProjectTemplatesQuery(User);
            var getProjectTemplatesResult = await _mediator.Send(getProjectTemplatesQuery);

            return getProjectTemplatesResult.IsSuccess
                ? Ok(getProjectTemplatesResult.Value)
                : BadRequest(getProjectTemplatesResult.Error);
        }

        [HttpDelete]
        [Route("{projectTemplateId:int}")]
        public async Task<IActionResult> Delete(int projectTemplateId)
        {
            var deleteProjectTemplateCommand = new DeleteProjectTemplateCommand(User, projectTemplateId);
            var deleteProjectTemplateResult = await _mediator.Send(deleteProjectTemplateCommand);

            return deleteProjectTemplateResult.IsSuccess
                ? Ok()
                : BadRequest();
        }

        private static CreateProjectTemplateCommand ToCommand(IPrincipal user, CreateProjectTemplateRequest createProjectTemplateRequest) =>
            new(
                user,
                createProjectTemplateRequest.Name,
                createProjectTemplateRequest.ClientVersion,
                createProjectTemplateRequest.Location
            );
    }
}
