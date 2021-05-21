using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace UOStudio.Server.Api.BackgroundJobs
{
    internal sealed class JobFactory : IJobFactory
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public JobFactory(
            ILogger logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger.ForContext<JobFactory>();
            _serviceProvider = serviceProvider;
        }

        public async Task<Maybe<IJob>> CreateJobAsync(JobDto job, CancellationToken cancellationToken)
        {
            var jobType = Type.GetType(job.JobType);
            if (jobType == null)
            {
                return Maybe<IJob>.None;
            }

            var jobPayloadType = Type.GetType(job.RequestPayloadType);
            if (jobPayloadType == null)
            {
                return Maybe<IJob>.None;
            }

            var jobPayload = JsonSerializer.Deserialize(job.RequestPayload, jobPayloadType);
            if (jobPayload == null)
            {
                return Maybe<IJob>.None;
            }

            var backgroundJob = _serviceProvider.GetRequiredService(jobType) as IJob;
            if (backgroundJob == null)
            {
                return Maybe<IJob>.None;
            }

            backgroundJob.Initialize(jobPayload);
            return Maybe<IJob>.From(backgroundJob);
        }
    }
}
