using System.Linq;
using Microsoft.EntityFrameworkCore;
using UOStudio.Core;
using UOStudio.Web.Data.Entities;

namespace UOStudio.Web.Data
{
    public sealed class UOStudioDbContext : DbContext
    {
        public UOStudioDbContext(DbContextOptions<UOStudioDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();

            if (!Users.Any())
            {
                var passwordHasher = new PasswordHasher();
                Users.Add(new User
                {
                    UserName = "admin",
                    PasswordHash = passwordHasher.Hash("admin"),
                    Permissions = Permissions.Viewer | Permissions.Editor | Permissions.Backup | Permissions.Administrator
                });
                Users.Add(new User
                {
                    UserName = "editor",
                    PasswordHash = passwordHasher.Hash("editor"),
                    Permissions = Permissions.Editor
                });
                Users.Add(new User
                {
                    UserName = "viewer",
                    PasswordHash = passwordHasher.Hash("viewer"),
                    Permissions = Permissions.Viewer
                });

                SaveChanges();
            }
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<TemporaryFile> TemporaryFiles { get; set; }
    }
}
