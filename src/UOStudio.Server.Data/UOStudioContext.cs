using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UOStudio.Common.Core;

namespace UOStudio.Server.Data
{
    public class UOStudioContext : DbContext
    {
        private readonly IPasswordHasher _passwordHasher;

        public UOStudioContext(
            IPasswordHasher passwordHasher,
            DbContextOptions<UOStudioContext> contextOptions
        )
            : base(contextOptions)
        {
            _passwordHasher = passwordHasher;
            EnsureCreated();
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<ProjectTemplate> ProjectTemplates { get; set; }

        public DbSet<BackgroundTask> BackgroundTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Project>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.CreatedBy);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Projects)
                .WithMany(p => p.AllowedUsers);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Permissions)
                .WithMany(p => p.Users);

            modelBuilder.Entity<BackgroundTask>()
                .HasKey(t => t.Id);
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

            var adminUser = await Users.FirstOrDefaultAsync(u => u.Name == "admin", CancellationToken.None);
            var userRex = await Users.FirstOrDefaultAsync(u => u.Name == "rex", CancellationToken.None);
            var userQuick = await Users.FirstOrDefaultAsync(u => u.Name == "quick", CancellationToken.None);
            var userBlocked = await Users.FirstOrDefaultAsync(u => u.Name == "blocked", CancellationToken.None);

            adminUser ??= new User
            {
                Name = "admin",
                Password = _passwordHasher.Hash("admin"),
                Permissions = Permission.AllPermissions.ToList()
            };
            userRex ??= new User
            {
                Name = "rex",
                Password = _passwordHasher.Hash("rex"),
                Permissions = Role.Editor.ToList()
            };
            userQuick ??= new User
            {
                Name = "quick",
                Password = _passwordHasher.Hash("quick"),
                Permissions = Role.Editor.ToList()
            };
            userBlocked ??= new User
            {
                Name = "blocked",
                Password = _passwordHasher.Hash("blocked"),
                Permissions = Role.BlockedUser.ToList()
            };

            await Users.AddRangeAsync(adminUser, userRex, userQuick, userBlocked);
            await SaveChangesAsync();

            ProjectTemplate projectTemplateVanilla5000 = null;
            ProjectTemplate projectTemplateVanilla7000 = null;
            ProjectTemplate projectTemplateVanilla708711 = null;

            projectTemplateVanilla5000 = new ProjectTemplate
            {
                Name = "Vanilla 5.0.0.0",
                ClientVersion = "5.0.0.0",
                Location = "D:\\UO\\Vanilla\\5.0.0.0\\5.0.0.0\\Server"
            };

            projectTemplateVanilla7000 = new ProjectTemplate
            {
                Name = "Vanilla 7.0.0.0",
                ClientVersion = "7.0.0.0",
                Location = @"D:\Private\Code\Projects\UOStudio\src\UOStudio.Server.Api\bin\Debug\ProjectTemplates\Vanilla-7.0.0.0\"
            };

            projectTemplateVanilla708711 = new ProjectTemplate
            {
                Name = "Vanilla 7.0.87.11",
                ClientVersion = "7.0.87.11",
                Location = "D:\\UO\\Vanilla\\7.0.87.11\\7.0.87.11\\Server"
            };

            await ProjectTemplates.AddRangeAsync(projectTemplateVanilla5000, projectTemplateVanilla7000, projectTemplateVanilla708711);
            await SaveChangesAsync();

            var privateProject5000 = new Project
            {
                Name = "Private Project 1",
                Description = "Some Private Project Rex is working on, based on 5.0.0.0",
                IsPublic = false,
                Template = projectTemplateVanilla5000,
                CreatedBy = userRex,
                CreatedAt = DateTimeOffset.Now,
                AllowedUsers = new List<User> { userQuick, userRex }
            };

            var privateProject7000 = new Project
            {
                Name = "Private Project 1",
                Description = "Some Private Project Rex is working on, based on 7.0.0.0",
                IsPublic = false,
                Template = projectTemplateVanilla7000,
                CreatedBy = userQuick,
                CreatedAt = DateTimeOffset.Now,
                AllowedUsers = new List<User> { userQuick, adminUser }
            };

            var publicProject = new Project
            {
                Name = "Public Project",
                Description = "Anybody can join",
                IsPublic = true,
                Template = projectTemplateVanilla708711,
                CreatedBy = adminUser,
                CreatedAt = DateTimeOffset.Now,
            };

            await Projects.AddRangeAsync(privateProject5000, privateProject7000, publicProject);
            await SaveChangesAsync();
        }
    }
}
