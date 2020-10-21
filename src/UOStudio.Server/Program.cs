using System;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UOStudio.Core.Settings;
using UOStudio.Server.Core.Settings;
using UOStudio.Server.Network;

namespace UOStudio.Server
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var serviceProvider = CreateCompositionRoot();

            var nedServer = serviceProvider.GetService<NedServer>();

            nedServer.Run();
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
            services.AddSingleton<IConfigurationLoader, ConfigurationLoader>();
            services.AddSingleton<IConfigurationSaver, ConfigurationSaver>();
            services.AddSingleton<IAppSettingsProvider, AppSettingsProvider>();
            services.AddSingleton<NedServer>();

            return services.BuildServiceProvider();
        }
    }
}
