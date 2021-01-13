using System;
using System.Net.Http.Headers;
using System.Net.Mime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UOStudio.Client.Core;
using UOStudio.Client.Core.Extensions;
using UOStudio.Client.Engine.Windows;
using UOStudio.Client.Network;
using UOStudio.Core;

namespace UOStudio.Client
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            //Environment.SetEnvironmentVariable("FNA3D_FORCE_DRIVER", "OpenGL");
            DllMap.Initialise();
            var serviceProvider = CreateServiceProvider();

            using var clientGame = serviceProvider.GetService<ClientGame>();
            clientGame!.Run();
        }

        private static IServiceProvider CreateServiceProvider()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File("client.log")
                .CreateLogger();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton(Log.Logger);
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<IFileVersionProvider, FileVersionProvider>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddTransient<INetworkClient, NetworkClient>();
            services.AddProfileService();
            services.AddSingleton<ClientGame>();
            AddHttpHandling(services);
            return services.BuildServiceProvider();
        }

        private static void AddHttpHandling(ServiceCollection services)
        {
            services.AddSingleton<AddBasicAuthenticationHandler>();
            services.AddHttpClient<INetworkClient, NetworkClient>(
                    client =>
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                    }
                )
                .AddHttpMessageHandler<AddBasicAuthenticationHandler>();
        }
    }
}
