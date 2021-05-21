using System;
using System.Threading;
using System.Threading.Tasks;

namespace UOStudio.Server.Api.BackgroundJobs
{
    public interface IJobDequeuer
    {
        Task<Guid> DequeueJobAsync(CancellationToken cancellationToken);
    }
}
