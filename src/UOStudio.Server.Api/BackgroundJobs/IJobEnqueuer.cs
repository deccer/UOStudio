using System;
using System.Threading;
using System.Threading.Tasks;

namespace UOStudio.Server.Api.BackgroundJobs
{
    public interface IJobEnqueuer
    {
        Task QueueJobAsync(Guid jobId, CancellationToken cancellationToken);
    }
}
