using Microsoft.Extensions.DependencyInjection;

namespace UOStudio.Server.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddLiteDb(this IServiceCollection services)
        {
            services.AddSingleton<ILiteDbFactory, LiteDbFactory>();
            services.AddSingleton<IUserRepository, UserRepository>();
        }
    }
}
