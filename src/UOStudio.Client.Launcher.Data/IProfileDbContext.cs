using Microsoft.EntityFrameworkCore;

namespace UOStudio.Client.Launcher.Data
{
    public interface IProfileDbContext : IDisposable
    {
        DbSet<Profile> Profiles { get; set; }

        Task MigrateAsync();
    }
}
