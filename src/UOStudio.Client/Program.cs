using System;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UOStudio.Client.Screens;
using UOStudio.Client.Services;
using UOStudio.Client.UI.Extensions;
using UOStudio.Common.Contracts;
using UOStudio.Common.Core;

namespace UOStudio.Client
{
    internal static class Program
    {
        private static IConfiguration _configuration;

        public static void Main(string[] args)
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            if (args.Length == 0)
            {
                Console.WriteLine("Unable to start client. Use the launcher.");
                return;
            }

            if (args.Length > 0)
            {
                var clientStartParametersEncoded = args[0];
                var clientStartParametersJson = Convert.FromBase64String(clientStartParametersEncoded);
                var clientStartParameters = JsonSerializer.Deserialize<ClientStartParameters>(clientStartParametersJson);
                if (clientStartParameters == null)
                {
                    Console.WriteLine("Unable to start client. Use the launcher. Illegal StartParameters");
                    Console.ReadLine();
                    return;
                }

                var connectionTicketEncoded = UrlEncodeBase64(clientStartParameters.ConnectionTicket);

                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(_configuration["Api:ApiEndpoint"]);
                httpClient.DefaultRequestHeaders.Clear();
                var validateTicketResponse = httpClient.GetAsync($"api/auth/validateTicket/{connectionTicketEncoded}").GetAwaiter().GetResult();
                if (!validateTicketResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine("Unable to start client. Use the launcher.");
                    Console.ReadLine();
                    return;
                }
            }

            DllMap.Initialise();
            using var serviceProvider = CreateServiceProvider();

            var graphicsSettings = serviceProvider.GetRequiredService<GraphicsSettings>();
            Environment.SetEnvironmentVariable("FNA3D_FORCE_DRIVER", graphicsSettings.Backend.ToString());

            var mainGame = serviceProvider.GetService<MainGame>();
            mainGame!.Run();
        }

        private static ServiceProvider CreateServiceProvider()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("client.log")
                .CreateLogger();

            var graphicsSettings = new GraphicsSettings();
            _configuration.GetSection(GraphicsSettings.Key).Bind(graphicsSettings);
            var clientSettings = new ClientSettings();
            _configuration.GetSection(ClientSettings.Key).Bind(clientSettings);

            var services = new ServiceCollection();
            services.AddSingleton(Log.Logger);
            services.AddSingleton(_configuration);
            services.AddSingleton(graphicsSettings);
            services.AddSingleton(clientSettings);
            services.AddSingleton<IScreenHandler, ScreenHandler>();
            services.AddSingleton<INetworkClient, NetworkClient>();
            services.AddSingleton<IProjectService, ProjectService>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IContext, Context>();
            services.AddWindows();
            services.AddSingleton<MainGame>();

            return services.BuildServiceProvider();
        }

        private static string UrlEncodeBase64(string base64Input)
        {
            return base64Input
                .Replace('+', '.')
                .Replace('/', '_')
                .Replace('=', '-');
        }
    }
}
