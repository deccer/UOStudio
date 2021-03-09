using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UOStudio.Client.Screens;
using UOStudio.Client.Services;
using UOStudio.Client.UI.Extensions;
using UOStudio.Common.Core;

namespace UOStudio.Client
{
    internal static class Program
    {
        public static void Main()
        {
            DllMap.Initialise();
            var serviceProvider = CreateServiceProvider();

            var graphicsSettings = serviceProvider.GetRequiredService<GraphicsSettings>();
            Environment.SetEnvironmentVariable("FNA3D_FORCE_DRIVER", graphicsSettings.Backend.ToString());

            var networkClient = serviceProvider.GetService<INetworkClient>();
            networkClient!.LoginSucceeded += (userId, projects) =>
            {
                Console.WriteLine(userId);
                Console.WriteLine(string.Join(", ", projects.Select(p => p.Name)));
            };
            networkClient!.LoginFailed += errorMessage =>
            {
                Console.WriteLine(errorMessage);
            };
            networkClient!.Connect(new Profile { UserName = "admin", Password = "admin" });

/*
            using var mainGame = serviceProvider.GetService<MainGame>();
            mainGame!.Run();
            */
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

            var graphicsSettings = new GraphicsSettings();
            configuration.GetSection(GraphicsSettings.Key).Bind(graphicsSettings);

            var services = new ServiceCollection();
            services.AddSingleton(Log.Logger);
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton(graphicsSettings);
            services.AddSingleton<IScreenHandler, ScreenHandler>();
            services.AddSingleton<INetworkClient, NetworkClient>();
            services.AddSingleton<IProfileService, ProfileService>();
            services.AddSingleton<IProjectService, ProjectService>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IContext, Context>();
            services.AddWindows();
            services.AddSingleton<MainGame>();
            services.AddHttpClient<NetworkClient>(client =>
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            });

            return services.BuildServiceProvider();
        }
    }
}
