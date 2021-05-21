using System;
using System.Threading;
using System.Threading.Tasks;

namespace UOStudio.Server.Api.BackgroundJobs
{
    public interface IJobCreator
    {
        Task<Guid> CreateBackgroundJobAsync<TJob, TJobPayload>(TJobPayload payload, CancellationToken cancellationToken)
            where TJob : IJob;
    }
}
