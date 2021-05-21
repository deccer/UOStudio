using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace UOStudio.Server.Api.BackgroundJobs
{
    internal sealed class JobDbContext : DbContext
    {
        public JobDbContext(
            DbContextOptions<JobDbContext> contextOptions)
            : base(contextOptions)
        {
            Database.EnsureCreated();
        }

        public DbSet<Job> Jobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
