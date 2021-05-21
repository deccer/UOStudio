using System;
using System.Threading;
using System.Threading.Tasks;

namespace UOStudio.Server.Api.BackgroundJobs
{
    public interface IJobUpdater
    {
        Task JobSucceededAsync<TResult>(Guid jobId, TResult result, CancellationToken cancellationToken);

        Task JobFailedAsync(Guid jobId, string errorMessage, CancellationToken cancellationToken);

        Task JobRunningAsync(Guid jobId, CancellationToken cancellationToken);
    }
}
