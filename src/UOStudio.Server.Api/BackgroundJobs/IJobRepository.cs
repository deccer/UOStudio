using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace UOStudio.Server.Api.BackgroundJobs
{
    internal interface IJobRepository
    {
        Task<Guid> CreateBackgroundJobAsync<TJob, TJobPayload>(TJobPayload jobPayload, CancellationToken cancellationToken)
            where TJob : IJob;

        Task<Maybe<JobDto>> GetBackgroundJobAsync(Guid jobId, CancellationToken cancellationToken);

        Task SetJobSucceededAsync<TResult>(Guid jobId, TResult result, CancellationToken cancellationToken);

        Task SetJobFailedAsync(Guid jobId, string errorMessage, CancellationToken cancellationToken);

        Task SetJobRunning(Guid jobId, CancellationToken cancellationToken);
    }
}
