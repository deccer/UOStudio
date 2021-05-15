using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace UOStudio.Client.Launcher.Data
{
    public sealed class ProfileDbContext : DbContext
    {
        public ProfileDbContext(DbContextOptions<ProfileDbContext> options)
            : base(options)
        {
            EnsureCreated();
        }
        
        public DbSet<Profile> Profiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProfileDbContext).Assembly);
        }
        
        [Conditional("DEBUG")]
        private void EnsureCreated()
        {
            EnsureCreatedCore().GetAwaiter().GetResult();
        }

        private async Task EnsureCreatedCore()
        {
            var created = await Database.EnsureCreatedAsync();
            if (!created)
            {
                return;
            }

            var localhostAdminProfile = await Profiles
                .FirstOrDefaultAsync(p => p.Name == "localhost - admin", CancellationToken.None);
            localhostAdminProfile ??= new Profile
            {
                Name = "localhost - admin",
                Description = "Local Development As Admin",
                ServerName = "localhost",
                ServerPort = 9050,
                UserName = "admin",
                Password = "admin",
                AuthBaseUri = "https://localhost:5001",
                ApiBaseUri = "https://localhost:5001"
            };
            var localhostEditorProfile = await Profiles
                .FirstOrDefaultAsync(p => p.Name == "localhost - editor", CancellationToken.None);
            localhostEditorProfile ??= new Profile
            {
                Name = "localhost - editor",
                Description = "Local Development As Editor",
                ServerName = "localhost",
                UserName = "editor",
                Password = "editor",
                AuthBaseUri = "https://localhost:5001",
                ApiBaseUri = "https://localhost:5001"
            };

            var localhostInvalidProfile = await Profiles
                .FirstOrDefaultAsync(p => p.Name == "localhost = invalid", CancellationToken.None);
            localhostInvalidProfile ??= new Profile
            {
                Name = "localhost - invalid",
                Description = "Local Development invalid profile, has no permissions",
                ServerName = "localhost",
                UserName = "invalid",
                Password = "invalid",
                AuthBaseUri = "https://localhost:5001",
                ApiBaseUri = "https://localhost:5001"
            };

            await Profiles.AddRangeAsync(localhostAdminProfile, localhostEditorProfile, localhostInvalidProfile);
            await SaveChangesAsync();
        }
    }
}