using System;
using Microsoft.Extensions.DependencyInjection;
using Ned.Server.Network;
using Serilog;

namespace Ned.Server
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var serviceProvider = CreateCompositionRoot();

            var nedServer = serviceProvider.GetService<NedServer>();

            nedServer.Run(9050);
        }

        private static IServiceProvider CreateCompositionRoot()
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("client.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var services = new ServiceCollection();
            services.AddSingleton<ILogger>(logger);
            services.AddSingleton<NedServer>();

            return services.BuildServiceProvider();
        }
    }
}
