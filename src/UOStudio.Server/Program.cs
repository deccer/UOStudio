using System;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UOStudio.Core;
using UOStudio.Server.Core.Settings;
using UOStudio.Server.Network;

namespace UOStudio.Server
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var serviceProvider = CreateCompositionRoot();

            var nedServer = serviceProvider.GetService<INetworkServer>();

            nedServer!.Run();
        }

        private static IServiceProvider CreateCompositionRoot()
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("client.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var services = new ServiceCollection();
            services.AddSingleton<ILogger>(logger);
            services.AddSingleton<ILoader, Loader>();
            services.AddSingleton<ISaver, Saver>();
            services.AddSingleton<IAppSettingsProvider, AppSettingsProvider>();
            services.AddNetworkServer();

            return services.BuildServiceProvider();
        }
    }
}
