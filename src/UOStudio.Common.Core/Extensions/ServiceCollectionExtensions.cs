using Microsoft.Extensions.DependencyInjection;

namespace UOStudio.Common.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPasswordHandling(this IServiceCollection services)
        {
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IPasswordVerifier, PasswordVerifier>();
        }
    }
}
