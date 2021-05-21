using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace UOStudio.Server.Api.BackgroundJobs
{
    internal sealed class JobHandler : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IJobDequeuer _jobDequeuer;
        private readonly IJobRepository _jobRepository;
        private readonly IJobFactory _jobFactory;
        private readonly IJobUpdater _jobUpdater;

        public JobHandler(
            ILogger logger,
            IJobDequeuer jobDequeuer,
            IJobRepository jobRepository,
            IJobFactory jobFactory,
            IJobUpdater jobUpdater)
        {
            _logger = logger.ForContext<JobHandler>();
            _jobDequeuer = jobDequeuer;
            _jobRepository = jobRepository;
            _jobFactory = jobFactory;
            _jobUpdater = jobUpdater;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Debug("BackgroundJobHandler Started");
            await ProcessJobsAsync(stoppingToken);
            _logger.Debug("BackgroundJobHandler Stopped");
        }

        private async Task ProcessJobsAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.Debug("Cancel Jobs Processing");
                    break;
                }

                var jobId = await _jobDequeuer.DequeueJobAsync(cancellationToken);
                var jobDto = await _jobRepository.GetBackgroundJobAsync(jobId, cancellationToken);
                if (jobDto.HasNoValue)
                {
                    _logger.Error("Job {@JobId} not found", jobId);
                    continue;
                }

                var job = await _jobFactory.CreateJobAsync(jobDto.Value, cancellationToken);
                if (job.HasNoValue)
                {
                    _logger.Error("Job {@JobId} unable to be materialized", jobId);
                    continue;
                }

                _logger.Debug("Job {@JobId} Processing {@RequestType}", jobId, job.GetType().Name);
                _jobUpdater.JobRunningAsync(jobId, cancellationToken);
                await job.Value.ProcessAsync(jobId, cancellationToken);
            }
        }
    }
}
