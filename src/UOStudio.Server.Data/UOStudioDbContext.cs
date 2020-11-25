using Microsoft.EntityFrameworkCore;

namespace UOStudio.Server.Data
{
    public class UOStudioDbContext : DbContext
    {
        public UOStudioDbContext(DbContextOptions<UOStudioDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; }

        public DbSet<Project> Projects { get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(account => account.Version)
                .IsRowVersion();
            modelBuilder.Entity<Project>()
                .Property(project => project.Version)
                .IsRowVersion();

            base.OnModelCreating(modelBuilder);
        }
    }
}
