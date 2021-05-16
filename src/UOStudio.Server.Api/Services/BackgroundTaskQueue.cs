using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using UOStudio.Server.Common;

namespace UOStudio.Server.Api.Services
{
    public sealed class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ILogger _logger;
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _workItems = new();
        private readonly SemaphoreSlim _signal = new(0);

        public BackgroundTaskQueue(ILogger logger)
        {
            _logger = logger;
        }

        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _logger.Debug("Queueing Task");
            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            if (_workItems.TryDequeue(out var workItem))
            {
                return workItem;
            }

            _logger.Error("BackgroundTaskQueue - Unable to dequeue work item");
            throw new InvalidOperationException();
        }
    }
}
