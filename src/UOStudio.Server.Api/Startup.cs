using System;
using System.IO;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using UOStudio.Common.Core.Extensions;
using UOStudio.Server.Api.Extensions;
using UOStudio.Server.Api.HostedServices;
using UOStudio.Server.Api.Services;
using UOStudio.Server.Common;
using UOStudio.Server.Data.Extensions;
using UOStudio.Server.Domain.GetProjects;
using UOStudio.Server.Services;

namespace UOStudio.Server.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .Enrich.WithCorrelationId()
                .Enrich.WithMachineName()
                .Enrich.WithMemoryUsage()
                .Enrich.WithCorrelationIdHeader()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "server.log"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            services.Configure<ServerSettings>(_configuration.GetSection(ServerSettings.ServerSection));
            services.Configure<SevenZipSettings>(_configuration.GetSection(SevenZipSettings.SevenZipSection));

            services.AddAsymmetricAuthentication(_configuration);

            services.AddSingleton(_configuration);
            services.AddSingleton(Log.Logger);
            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "UOStudio.Server.Api", Version = "v1" }); });
            services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblies(typeof(GetProjectsQuery).Assembly); });

            services.AddLiteDb();
            services.AddPasswordHandling();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IUserService, UserService>();

            services.AddSingleton<IProjectService, ProjectService>();
            services.AddSingleton<IProjectTemplateService, ProjectTemplateService>();

            services.AddSingleton<ISevenZipService, SevenZipService>();
            services.AddSingleton<ICommandRunner, CommandRunner>();

            services.AddSingleton<INetworkServer, NetworkServer>();
            services.AddHostedService<MapEditService>();

            services.AddBackgroundJobs();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSerilogRequestLogging();
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UOStudio.Server.Api v1"));
        }
    }
}
