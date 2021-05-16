using System;
using System.IO;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using UOStudio.Common.Core;
using UOStudio.Server.Api.Extensions;
using UOStudio.Server.Api.HostedServices;
using UOStudio.Server.Api.Services;
using UOStudio.Server.Common;
using UOStudio.Server.Data;
using UOStudio.Server.Domain.GetProjectDetailsByName;
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

            var serverSettings = new ServerSettings();
            _configuration.GetSection("Server").Bind(serverSettings);

            var sevenZipOptions = new SevenZipOptions();
            _configuration.GetSection("SevenZip").Bind(sevenZipOptions);

            services.AddAsymmetricAuthentication(_configuration);

            services.AddSingleton(_configuration);
            services.AddSingleton(Log.Logger);
            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "UOStudio.Server.Api", Version = "v1" }); });
            services.AddMediatR(typeof(GetProjectDetailsByNameQueryHandler).Assembly);
            services.AddDbContextFactory<UOStudioContext>((provider, builder) =>
                {
                    var ss = provider.GetRequiredService<ServerSettings>();
                    builder.UseSqlite(string.IsNullOrEmpty(Path.GetDirectoryName(ss.DatabaseDirectory))
                        ? $"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", ss.DatabaseDirectory)}"
                        : $"Data Source={ss.DatabaseDirectory}");
                }
            );
            services.AddSingleton(serverSettings);
            services.AddSingleton(sevenZipOptions);
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IPasswordVerifier, PasswordVerifier>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IUserService, UserService>();

            services.AddSingleton<IProjectService, ProjectService>();
            services.AddSingleton<IProjectTemplateService, ProjectTemplateService>();
            services.AddSingleton<ISevenZipService, SevenZipService>();
            services.AddSingleton<ICommandRunner, CommandRunner>();

            services.AddSingleton<INetworkServer, NetworkServer>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<MapEditService>();
            services.AddHostedService<QueuedHostedService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<UOStudioContext>>();
                using var context = contextFactory.CreateDbContext();
                context.Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                app.UseSerilogRequestLogging();
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UOStudio.Server.Api v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
