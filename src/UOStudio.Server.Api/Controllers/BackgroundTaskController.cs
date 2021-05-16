using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UOStudio.Server.Data;

namespace UOStudio.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BackgroundTaskController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IDbContextFactory<UOStudioContext> _contextFactory;

        public BackgroundTaskController(
            ILogger logger,
            IDbContextFactory<UOStudioContext> contextFactory)
        {
            _logger = logger.ForContext<BackgroundTaskController>();
            _contextFactory = contextFactory;
        }

        [HttpGet("{backgroundTaskId:guid}", Name = nameof(GetBackgroundTask))]
        public async Task<IActionResult> GetBackgroundTask(Guid backgroundTaskId)
        {
            await using var db = _contextFactory.CreateDbContext();

            var backgroundTask = await db.BackgroundTasks.FirstOrDefaultAsync(bt => bt.Id == backgroundTaskId);
            return backgroundTask.Status switch
            {
                BackgroundTaskStatus.Running => AcceptedAtRoute(nameof(GetBackgroundTask), new { backgroundTaskId }),
                BackgroundTaskStatus.Failed => BadRequest(backgroundTask),
                _ => RedirectToRoute(nameof(DownloadController.GetDownload), new { downloadId = backgroundTask.Id })
            };
        }
    }
}
