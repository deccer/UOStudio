using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace UOStudio.Server.Api.BackgroundJobs
{
    internal sealed class JobCreator : IJobCreator
    {
        private readonly ILogger _logger;
        private readonly IJobRepository _jobRepository;

        public JobCreator(
            ILogger logger,
            IJobRepository jobRepository)
        {
            _logger = logger.ForContext<JobCreator>();
            _jobRepository = jobRepository;
        }

        public Task<Guid> CreateBackgroundJobAsync<TJob, TJobPayload>(TJobPayload payload, CancellationToken cancellationToken)
            where TJob : IJob =>
            _jobRepository.CreateBackgroundJobAsync<TJob, TJobPayload>(payload, cancellationToken);
    }
}
