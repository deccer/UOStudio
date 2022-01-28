using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UOStudio.Client.Launcher.Contracts;
using UOStudio.Client.Launcher.Data.Repositories;

namespace UOStudio.Client.Launcher.Data
{
    public static class ServiceCollectionExtensions
    {
        public static void AddProfileDatabase(this IServiceCollection services)
        {
            services.AddDbContextFactory<ProfileDbContext>((provider, options) =>
            {
                var cs = provider.GetRequiredService<ClientSettings>();
                var profilesDb = string.IsNullOrEmpty(Path.GetDirectoryName(cs.ProfilesDirectory))
                    ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Profiles", cs.ProfilesDirectory)
                    : cs.ProfilesDirectory;
                var profilesDbDirectory = Path.GetDirectoryName(profilesDb);
                Directory.CreateDirectory(profilesDbDirectory);

                options.UseSqlite($"Data Source={profilesDb}");
            });
            services.AddSingleton<IProfileRepository, ProfileRepository>();
        }
    }
}
