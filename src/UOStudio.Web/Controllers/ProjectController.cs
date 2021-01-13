using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UOStudio.Core;
using UOStudio.Web.Contracts;
using UOStudio.Web.Models;
using UOStudio.Web.Services;

namespace UOStudio.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : BaseController
    {
        private readonly IProjectService _projectService;

        public ProjectController(
            IProjectService projectService) =>
            _projectService = projectService;

        [HttpGet]
        public Task<IEnumerable<ProjectDto>> Get() => _projectService.GetAllAsync();

        [HttpGet("{projectId}")]
        public async Task<IActionResult> Get([FromRoute] Guid projectId)
        {
            var userId = GetUserId();
            var result = await _projectService.GetProjectAsync(projectId, userId);
            if (result.IsFailure)
            {
                // TODO: handle
            }

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateProjectModel project)
        {
            var userId = GetUserId();
            if (!User.HasClaim(ClaimTypes.Role, nameof(Permissions.Administrator)))
            {
                return Forbid();
            }

            var result = await _projectService.CreateProjectAsync(project.Name, project.Description, project.ClientVersion, userId);
            return result.IsFailure
                ? Problem($"Unable to create project {project.Name}")
                : CreatedAtRoute("GetFile", new { projectId = result.Value }, result.Value);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] Guid projectId)
        {
            var userId = GetUserId();
            if (!User.HasClaim(ClaimTypes.Role, nameof(Permissions.Administrator)))
            {
                return Forbid();
            }

            var result = await _projectService.DeleteProjectAsync(projectId, userId);
            return result.IsFailure
                ? Problem($"Unable to delete project {projectId:N}")
                : Ok();
        }

        [HttpPost]
        [Route("join")]
        public async Task<IActionResult> Join([FromRoute] Guid projectId)
        {
            var userId = GetUserId();

            var result = 9;

            return Ok();
        }
    }
}
