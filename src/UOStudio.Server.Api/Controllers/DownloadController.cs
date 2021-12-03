using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using UOStudio.Server.Common;

namespace UOStudio.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DownloadController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ServerSettings _serverSettings;

        public DownloadController(
            ILogger logger,
            IOptions<ServerSettings> serverSettings)
        {
            _logger = logger.ForContext<DownloadController>();
            _serverSettings = serverSettings.Value;
        }

        [HttpGet("{downloadId:guid}", Name = nameof(GetDownload))]
        public async Task<IActionResult> GetDownload(Guid downloadId)
        {
            var downloadFilePath = string.IsNullOrEmpty(Path.GetDirectoryName(_serverSettings.DownloadsDirectory))
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _serverSettings.DownloadsDirectory, $"{downloadId.ToString()}.7z")
                : Path.Combine(_serverSettings.DownloadsDirectory, $"{downloadId.ToString()}.7z");

            var downloadStream = System.IO.File.OpenRead(downloadFilePath);
            return File(downloadStream, MediaTypeNames.Application.Zip);
        }
    }
}
