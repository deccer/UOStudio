using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace UOStudio.Server.Api.BackgroundJobs
{
    public interface IJobFactory
    {
        Task<Maybe<IJob>> CreateJobAsync(JobDto job, CancellationToken cancellationToken);
    }
}
