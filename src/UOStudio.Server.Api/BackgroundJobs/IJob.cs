using System;
using System.Threading;
using System.Threading.Tasks;

namespace UOStudio.Server.Api.BackgroundJobs
{
    public interface IJob
    {
        void Initialize(object payload);

        Task ProcessAsync(Guid jobId, CancellationToken cancellationToken);
    }
}
