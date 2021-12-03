using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using UOStudio.Client.Services;
using UOStudio.Client.UI.Extensions;
using UOStudio.Client.Worlds;
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

            ClientStartParameters clientStartParameters = new ClientStartParameters
            {
                ProjectId = 1,
            };
#if RELEASE
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
                    Console.WriteLine("Illegal StartParameters, unable to start client. Use the launcher.");
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
#endif

            DllMap.Initialise();
            using var serviceProvider = CreateServiceProvider(clientStartParameters);

            var graphicsSettings = serviceProvider.GetRequiredService<IOptions<GraphicsSettings>>();
            Environment.SetEnvironmentVariable("FNA3D_FORCE_DRIVER", graphicsSettings.Value.Backend.ToString());

            using var mainGame = serviceProvider.GetRequiredService<MainGame>();
            mainGame.Run();
        }

        private static ServiceProvider CreateServiceProvider(ClientStartParameters clientStartParameters)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("client.log")
                .CreateLogger();

            var services = new ServiceCollection();
            services.AddSingleton(clientStartParameters);
            services.Configure<GraphicsSettings>(_configuration.GetSection(GraphicsSettings.Key));
            services.Configure<ClientSettings>(_configuration.GetSection(ClientSettings.Key));
            services.AddSingleton(Log.Logger);
            services.AddSingleton(_configuration);
            services.AddSingleton<INetworkClient, NetworkClient>();
            services.AddSingleton<IProjectService, ProjectService>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IContext, Context>();
            services.AddWindows();
            services.AddSingleton<IWorldProvider, WorldProvider>();
            services.AddSingleton<IWorldRenderer, WorldRenderer>();
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
