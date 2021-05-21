using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Serilog;

namespace UOStudio.Server.Api.BackgroundJobs
{
    internal sealed class JobEnqueuer : IJobEnqueuer
    {
        private readonly ILogger _logger;
        private readonly Channel<Guid> _channel;

        public JobEnqueuer(
            ILogger logger,
            Channel<Guid> channel)
        {
            _logger = logger.ForContext<JobEnqueuer>();
            _channel = channel;
        }

        public async Task QueueJobAsync(Guid jobId, CancellationToken cancellationToken)
        {
            while (await _channel.Writer.WaitToWriteAsync(cancellationToken))
            {
                if (_channel.Writer.TryWrite(jobId))
                {
                    _logger.Debug("Job {@JobId} Queued", jobId);
                    return;
                }
            }
        }
    }
}
