using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace UOStudio.Server.Api.Extensions
{
    public static class AuthenticationExtensions
    {
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
