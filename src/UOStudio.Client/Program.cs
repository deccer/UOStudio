using System;
using System.Net.Http.Headers;
using System.Net.Mime;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UOStudio.Client.Core;
using UOStudio.Client.Core.Settings;
using UOStudio.Client.Engine;
using UOStudio.Client.Engine.Windows;
using UOStudio.Client.Network;
using UOStudio.Core;

namespace UOStudio.Client
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("FNA3D_FORCE_DRIVER", "OpenGL");
            DllMap.Initialise();
            var compositionRoot = CreateCompositionRoot();

            using var clientGame = compositionRoot.GetService<ClientGame>();
            clientGame!.Run();
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
            services.AddSingleton<IFileVersionProvider, FileVersionProvider>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<AddBasicAuthenticationHandler>();
            services.AddTransient<INetworkClient, NetworkClient>();
            services.AddHttpClient<INetworkClient, NetworkClient>(
                    client =>
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                    }
                )
                .AddHttpMessageHandler<AddBasicAuthenticationHandler>();
            services.AddSingleton<ProfileService>();
            services.AddSingleton<ClientGame>();
            return services.BuildServiceProvider();
        }
    }
}
