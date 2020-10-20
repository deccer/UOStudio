using System;
using Microsoft.Extensions.DependencyInjection;
using Ned.Client.Engine;
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
            var services = new ServiceCollection();
            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            services.AddSingleton<ILogger>(logger);
            services.AddSingleton<ClientGame>();
            return services.BuildServiceProvider();
        }
    }
}
