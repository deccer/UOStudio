using Microsoft.Extensions.DependencyInjection;

namespace UOStudio.Client.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddProfileService(this ServiceCollection services)
        {
            services.AddSingleton<IProfileLoader, ProfileLoader>();
            services.AddSingleton<IProfileSaver, ProfileSaver>();
            services.AddSingleton<IProfileService, ProfileService>();
        }
    }
}
