using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using UOStudio.Server.Api.Models;

namespace UOStudio.Server.Api.BackgroundJobs.Jobs
{
    internal sealed class CreateProjectJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IJobUpdater _jobUpdater;

        private string _name;
        private string _description;
        private string _template;

        public CreateProjectJob(
            ILogger logger,
            IJobUpdater jobUpdater)
        {
            _logger = logger.ForContext<CreateProjectJob>();
            _jobUpdater = jobUpdater;
        }

        public void Initialize(object payload)
        {
            var p = (CreateProjectDto)payload;
            _name = p.Name;
            _description = p.Description;
            _template = p.Template;
        }

        public async Task ProcessAsync(Guid jobId, CancellationToken cancellationToken)
        {
            var random = new Random();
            await Task.Delay(random.Next(3000, 10000), cancellationToken);
            if (random.Next(100) < 50)
            {
                await _jobUpdater.JobFailedAsync(jobId, "Job Failed", cancellationToken);
            }
            else
            {
                var x = new { Xo = 234, Fr = $"Project {_name} created" };
                await _jobUpdater.JobSucceededAsync(jobId, x, cancellationToken);
            }
        }
    }
}
