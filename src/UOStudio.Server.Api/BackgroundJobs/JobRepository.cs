using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace UOStudio.Server.Api.BackgroundJobs
{
    internal sealed class JobRepository : IJobRepository
    {
        private readonly ILogger _logger;
        private readonly IDbContextFactory<JobDbContext> _contextFactory;

        public JobRepository(
            ILogger logger,
            IDbContextFactory<JobDbContext> contextFactory)
        {
            _logger = logger;
            _contextFactory = contextFactory;
        }

        public async Task<Guid> CreateBackgroundJobAsync<TJob, TJobPayload>(TJobPayload jobPayload, CancellationToken cancellationToken)
            where TJob : IJob
        {
            await using var context = _contextFactory.CreateDbContext();
            var job = new Job
            {
                Status = JobStatus.Created,
                StatusDate = DateTime.UtcNow,
                JobType = typeof(TJob).FullName,
                RequestPayload = JsonSerializer.Serialize(jobPayload),
                RequestPayloadType = typeof(TJobPayload).FullName
            };

            await context.Jobs.AddAsync(job, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return job.Id;
        }

        public async Task<Maybe<JobDto>> GetBackgroundJobAsync(Guid jobId, CancellationToken cancellationToken)
        {
            await using var context = _contextFactory.CreateDbContext();
            var job = await context.Jobs.FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
            if (job != null)
            {
                return ToDto(job);
            }

            _logger.Error("Job {@JobId} not found", jobId);
            return null;
        }

        public async Task SetJobSucceededAsync<TResult>(Guid jobId, TResult result, CancellationToken cancellationToken)
        {
            await using var context = _contextFactory.CreateDbContext();
            var job = await context.Jobs.FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
            if (job != null)
            {
                job.Status = JobStatus.Succeeded;
                job.StatusDate = DateTime.UtcNow;
                job.ResultType = typeof(TResult).FullName;
                job.ResultPayload = JsonSerializer.Serialize(result);

                context.Jobs.Update(job);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task SetJobFailedAsync(Guid jobId, string errorMessage, CancellationToken cancellationToken)
        {
            await using var context = _contextFactory.CreateDbContext();
            var job = await context.Jobs.FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
            if (job != null)
            {
                job.Status = JobStatus.Failed;
                job.StatusDate = DateTime.UtcNow;
                job.ResultPayload = errorMessage;

                context.Jobs.Update(job);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task SetJobRunning(Guid jobId, CancellationToken cancellationToken)
        {
            await using var context = _contextFactory.CreateDbContext();
            var job = await context.Jobs.FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
            if (job != null)
            {
                job.Status = JobStatus.Running;
                job.StatusDate = DateTime.UtcNow;

                context.Jobs.Update(job);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        private JobDto ToDto(Job job)
        {
            return new JobDto(job.Id, job.JobType, job.RequestPayload, job.RequestPayloadType);
        }
    }
}
