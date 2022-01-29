using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ILiteDbFactory _liteDbFactory;

        public BackgroundTaskController(
            ILogger logger,
            ILiteDbFactory liteDbFactory)
        {
            _logger = logger.ForContext<BackgroundTaskController>();
            _liteDbFactory = liteDbFactory;
        }

        [HttpGet("{backgroundTaskId:guid}", Name = nameof(GetBackgroundTask))]
        public async Task<IActionResult> GetBackgroundTask(Guid backgroundTaskId)
        {
            using var db = _liteDbFactory.CreateLiteDatabase();

            var backgroundTasks = db.GetCollection<BackgroundTask>(nameof(BackgroundTask));
            var backgroundTask = await backgroundTasks.FindOneAsync(bt => bt.Id == backgroundTaskId);
            return backgroundTask.Status switch
            {
                BackgroundTaskStatus.Running => AcceptedAtRoute(nameof(GetBackgroundTask), new { backgroundTaskId }),
                BackgroundTaskStatus.Failed => BadRequest(backgroundTask),
                _ => RedirectToRoute(nameof(DownloadController.GetDownload), new { downloadId = backgroundTask.Id })
            };
        }
    }
}
