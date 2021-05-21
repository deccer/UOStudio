using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Serilog;

namespace UOStudio.Server.Api.BackgroundJobs
{
    internal sealed class JobDequeuer : IJobDequeuer
    {
        private readonly ILogger _logger;
        private readonly Channel<Guid> _channel;

        public JobDequeuer(
            ILogger logger,
            Channel<Guid> channel)
        {
            _logger = logger.ForContext<JobDequeuer>();
            _channel = channel;
        }

        public async Task<Guid> DequeueJobAsync(CancellationToken cancellationToken)
        {
            while (await _channel.Reader.WaitToReadAsync(cancellationToken))
            {
                if (_channel.Reader.TryRead(out var jobId))
                {
                    _logger.Debug("Job {@JobId} Found", jobId);
                    return jobId;
                }
            }

            return Guid.Empty;
        }
    }
}
