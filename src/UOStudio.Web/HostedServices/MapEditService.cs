using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;
using UOStudio.Web.Network;

namespace UOStudio.Web.HostedServices
{
    public class MapEditService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly INetworkServer _networkServer;

        public MapEditService(
            ILogger logger,
            IHostApplicationLifetime hostApplicationLifetime,
            INetworkServer networkServer)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _networkServer = networkServer;
            _logger = logger.ForContext<MapEditService>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _hostApplicationLifetime.ApplicationStarted.Register(OnApplicationStarted);
            _hostApplicationLifetime.ApplicationStopping.Register(OnApplicationStopping);
            _hostApplicationLifetime.ApplicationStopped.Register(OnApplicationStopped);
            _logger.Debug("Starting MapEditService...");
            _logger.Debug("Starting MapEditService...Done");

            return Task.CompletedTask;
        }

        private void OnApplicationStarted()
        {
            _logger.Debug("AppStarted");
            _networkServer.Start(9050);
        }

        private void OnApplicationStopping()
        {
            _logger.Debug("AppStopping");
            _networkServer.Stop();
        }

        private void OnApplicationStopped()
        {
            _logger.Debug("AppStopped");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Debug("Stopping MapEditService...");
            _logger.Debug("Stopping MapEditService...Done");

            return Task.CompletedTask;
        }
    }
}
