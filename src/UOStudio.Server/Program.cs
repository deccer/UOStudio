using System;
using System.Reflection;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UOStudio.Common.Contracts;
using UOStudio.Common.Core;
using UOStudio.Server.Common;
using UOStudio.Server.Data;
using UOStudio.Server.Services;

namespace UOStudio.Server
{
    internal static class Program
    {
        public static void Main()
        {
            var serviceProvider = BuildServiceProvider();

            var uoStudioServer = serviceProvider.GetService<NetworkServer>();

            uoStudioServer!.Run();
        }

        private static IServiceProvider BuildServiceProvider()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.File("server.log")
                .WriteTo.Console()
                .MinimumLevel.Verbose()
                .CreateLogger();

            var services = new ServiceCollection();

            var serverSettings = new ServerSettings();
            configuration.GetSection("Server").Bind(serverSettings);
            services.AddSingleton(serverSettings);
            services.AddSingleton(Log.Logger);
            services.AddSingleton<NetworkServer>();
            services.AddMediatR(typeof(Program).GetTypeInfo().Assembly);
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IPasswordVerifier, PasswordVerifier>();
            services.AddSingleton<ICommandRunner, CommandRunner>();
            services.AddSingleton<IGitClient, GitClient>();
            services.AddSingleton<IProjectTemplateService, ProjectTemplateService>();
            services.AddAutoMapper(mapperConfiguration => ConfigureAutoMapper(mapperConfiguration));
            return services.BuildServiceProvider();
        }

        private static void ConfigureAutoMapper(IMapperConfigurationExpression mapperConfiguration)
        {
            mapperConfiguration.CreateMap<Project, ProjectDto>();
        }
    }
}
