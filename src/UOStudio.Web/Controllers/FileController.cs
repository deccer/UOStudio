using System;
using System.IO;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UOStudio.Web.Data;
using UOStudio.Web.Services;

namespace UOStudio.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ITemporaryFileService _temporaryFileService;

        public FileController(
            ILogger logger,
            ITemporaryFileService temporaryFileService)
        {
            _logger = logger;
            _temporaryFileService = temporaryFileService;
        }

        [HttpGet("{projectId}", Name = "GetFile")]
        public async Task<IActionResult> Get([FromRoute] Guid projectId)
        {
            _logger.Debug($"Requesting client.zip for project {projectId}...");
            var result = await _temporaryFileService
                .GetFileAsync(projectId)
                .ConfigureAwait(false);
            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }

            return File(result.Value, MediaTypeNames.Application.Zip);
        }
    }
}
