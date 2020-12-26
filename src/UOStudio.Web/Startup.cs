using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using UOStudio.Core;
using UOStudio.Web.Data;
using UOStudio.Web.Handlers;
using UOStudio.Web.HostedServices;
using UOStudio.Web.Network;
using UOStudio.Web.Services;

namespace UOStudio.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) => _configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();

            services.Configure<ProjectOptions>(_configuration.GetSection(ProjectOptions.ProjectSettings));

            services.AddSingleton<ILogger>(logger);
            services.AddDatabase(options => options.UseSqlite("Data Source=uostudio.db"));
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IPasswordVerifier, PasswordVerifier>();
            services.AddSingleton<ITemporaryFileService, TemporaryFileService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IProjectService, ProjectService>();

            services.AddCors();
            services.AddControllers();
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "UOStudio.Web",
                    Version = "v1"
                });
            });

            services.AddHostedService<MapEditService>();
            services.AddNetworkServer();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            env.WebRootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UOStudio.Web v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
