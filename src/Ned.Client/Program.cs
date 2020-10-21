using System;
using Microsoft.Extensions.DependencyInjection;
using Ned.Client.Engine;
using Ned.Client.Network;
using Serilog;

namespace Ned.Client
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
            services.AddSingleton<INedClient, NedClient>();
            services.AddSingleton<ClientGame>();
            return services.BuildServiceProvider();
        }
    }
}
