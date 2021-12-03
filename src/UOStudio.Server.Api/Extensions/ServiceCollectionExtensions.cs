using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UOStudio.Server.Api.BackgroundJobs;
using UOStudio.Server.Api.BackgroundJobs.Jobs;
using UOStudio.Server.Common;
using UOStudio.Server.Services;

namespace UOStudio.Server.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBackgroundJobs(this IServiceCollection services)
        {
            services.AddDbContextFactory<JobDbContext>((provider, builder) =>
            {
                var ss = provider.GetRequiredService<IOptions<ServerSettings>>();
                var serverSettings = ss.Value;
                var databaseDirectory = string.IsNullOrEmpty(Path.GetDirectoryName(serverSettings.DatabaseDirectory))
                    ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, serverSettings.DatabaseDirectory)
                    : serverSettings.DatabaseDirectory;
                Directory.CreateDirectory(databaseDirectory);
                builder.UseSqlite($"Data Source={Path.Combine(databaseDirectory, "Jobs.db")}");
            });
            services.AddSingleton<IJobCreator, JobCreator>();
            services.AddSingleton<IJobDequeuer, JobDequeuer>();
            services.AddSingleton<IJobEnqueuer, JobEnqueuer>();
            services.AddSingleton<IJobUpdater, JobUpdater>();
            services.AddSingleton<IJobRepository, JobRepository>();
            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton(_ => Channel.CreateUnbounded<Guid>(new UnboundedChannelOptions { SingleReader = true }));
            services.AddSingleton<IProjectService, ProjectService>();

            services.AddSingleton<CreateProjectJob>();
            services.AddHostedService<JobHandler>();
        }

        public static IServiceCollection AddAsymmetricAuthentication(
            this IServiceCollection services, IConfiguration configuration)
        {
            var publicKey = Convert.FromBase64String(configuration["JwtToken:RsaPublicKey"]);
            var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(publicKey, out _);

            var issuer = configuration["JwtToken:Issuer"];
            var audience = configuration["JwtToken:Audience"];

            services.AddAuthentication(authOptions =>
                {
                    authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        RequireAudience = true,
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,

                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        ValidAudiences = new [] { audience },
                        IssuerSigningKey = new RsaSecurityKey(rsa),
                        CryptoProviderFactory = new CryptoProviderFactory
                        {
                            CacheSignatureProviders = false
                        },
                        LifetimeValidator = LifetimeValidator,

                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = x =>
                        {
                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }

        private static bool LifetimeValidator(DateTime? notBefore,
            DateTime? expires,
            SecurityToken securityToken,
            TokenValidationParameters validationParameters)
        {
            return expires != null && expires > DateTime.UtcNow;
        }
    }
}
