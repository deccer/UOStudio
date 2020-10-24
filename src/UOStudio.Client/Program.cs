using System;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UOStudio.Client.Core.Settings;
using UOStudio.Client.Engine;
using UOStudio.Client.Network;
using UOStudio.Core;

namespace UOStudio.Client
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            DllMap.Initialise();
            var compositionRoot = CreateCompositionRoot();

            using var clientGame = compositionRoot.GetService<ClientGame>();
            clientGame.Run();
        }

        private static IServiceProvider CreateCompositionRoot()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File("client.log")
                .CreateLogger();

            var services = new ServiceCollection();
            services.AddSingleton<ILogger>(Log.Logger);
            services.AddSingleton<ILoader, Loader>();
            services.AddSingleton<ISaver, Saver>();
            services.AddSingleton<IAppSettingsProvider, AppSettingsProvider>();
            services.AddSingleton<INetworkClient, NetworkClient>();
            services.AddSingleton<ClientGame>();
            return services.BuildServiceProvider();
        }
    }
}
