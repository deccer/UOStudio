using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;
using UOStudio.Server.Common;

namespace UOStudio.Server.Api.HostedServices
{
    public sealed class QueuedHostedService : BackgroundService
    {
        private readonly ILogger _logger;

        public QueuedHostedService(IBackgroundTaskQueue taskQueue, ILogger logger)
        {
            TaskQueue = taskQueue;
            _logger = logger.ForContext<QueuedHostedService>();
        }

        public IBackgroundTaskQueue TaskQueue { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Information("QueuedHostedService - Starting");

            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(stoppingToken);

                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error occurred executing {@WorkItem}", nameof(workItem));
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.Information("QueuedHostedService - Stopping");

            await base.StopAsync(stoppingToken);
        }
    }
}
