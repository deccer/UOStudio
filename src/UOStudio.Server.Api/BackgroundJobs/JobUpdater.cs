using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace UOStudio.Server.Api.BackgroundJobs
{
    internal sealed class JobUpdater : IJobUpdater
    {
        private readonly ILogger _logger;
        private readonly IJobRepository _jobRepository;

        public JobUpdater(
            ILogger logger,
            IJobRepository jobRepository)
        {
            _logger = logger;
            _jobRepository = jobRepository;
        }

        public async Task JobSucceededAsync<TResult>(Guid jobId, TResult result, CancellationToken cancellationToken)
        {
            await _jobRepository.SetJobSucceededAsync(jobId, result, cancellationToken);
            _logger.Debug("Job {@JobId} Succeeded", jobId);
        }

        public async Task JobFailedAsync(Guid jobId, string errorMessage, CancellationToken cancellationToken)
        {
            await _jobRepository.SetJobFailedAsync(jobId, errorMessage, cancellationToken);
            _logger.Debug("Job {@JobId} Failed", jobId);
        }

        public async Task JobRunningAsync(Guid jobId, CancellationToken cancellationToken)
        {
            await _jobRepository.SetJobRunning(jobId, cancellationToken);
            _logger.Debug("Job {@JobId} Running", jobId);
        }
    }
}
