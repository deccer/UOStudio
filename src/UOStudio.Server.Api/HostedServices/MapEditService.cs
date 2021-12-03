using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using UOStudio.Server.Common;

namespace UOStudio.Server.Api.HostedServices
{
    public sealed class MapEditService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly INetworkServer _networkServer;
        private readonly ServerSettings _serverSettings;

        public MapEditService(
            ILogger logger,
            IHostApplicationLifetime hostApplicationLifetime,
            INetworkServer networkServer,
            IOptions<ServerSettings> serverSettings)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _networkServer = networkServer;
            _serverSettings = serverSettings.Value;
            _logger = logger.ForContext<MapEditService>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Debug("MapEditService Starting...");
            _hostApplicationLifetime.ApplicationStarted.Register(OnApplicationStarted);
            _hostApplicationLifetime.ApplicationStopping.Register(OnApplicationStopping);
            _logger.Debug("MapEditService Starting...Done");

            return Task.CompletedTask;
        }

        private void OnApplicationStarted()
        {
            _networkServer.Start(_serverSettings.Port);
        }

        private void OnApplicationStopping()
        {
            _networkServer.Stop();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
