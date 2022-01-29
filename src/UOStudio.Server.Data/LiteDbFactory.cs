using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using LiteDB.Async;
using Microsoft.Extensions.Options;
using Serilog;
using UOStudio.Common.Core;
using UOStudio.Server.Common;

namespace UOStudio.Server.Data
{
    internal sealed class LiteDbFactory : ILiteDbFactory
    {
        private readonly ILogger _logger;
        private readonly IPasswordHasher _passwordHasher;
        private readonly string _databaseFilePath;

        public LiteDbFactory(
            ILogger logger,
            IOptions<ServerSettings> serverSettings,
            IPasswordHasher passwordHasher)
        {
            _logger = logger;
            _passwordHasher = passwordHasher;

            var databaseDirectory = string.IsNullOrEmpty(Path.GetDirectoryName(serverSettings.Value.DatabaseDirectory))
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, serverSettings.Value.DatabaseDirectory)
                : serverSettings.Value.DatabaseDirectory;
            Directory.CreateDirectory(databaseDirectory);
            _databaseFilePath = Path.Combine(databaseDirectory, "UOStudio.litedb");

            MapEntities();
        }

        public ILiteDatabaseAsync CreateLiteDatabase()
        {
            ILiteDatabaseAsync db;
            if (!File.Exists(_databaseFilePath))
            {
                db = new LiteDatabaseAsync(_databaseFilePath);
                SeedDatabase(db).GetAwaiter().GetResult();
            }
            else
            {
                db = new LiteDatabaseAsync(_databaseFilePath);
            }

            return db;
        }

        private static void MapEntities()
        {
            BsonMapper.Global.Entity<User>()
                .Id(user => user.Id);

            BsonMapper.Global.Entity<ProjectTemplate>()
                .Id(projectTemplate => projectTemplate.Id)
                .DbRef(projectTemplate => projectTemplate.CreatedBy);

            BsonMapper.Global.Entity<Project>()
                .Id(project => project.Id)
                .DbRef(project => project.CreatedBy)
                .DbRef(project => project.Template)
                .DbRef(project => project.AllowedUsers, nameof(User));

            BsonMapper.Global.Entity<BackgroundTask>()
                .Id(backgroundTask => backgroundTask.Id);
        }

        private async Task SeedDatabase(ILiteDatabaseAsync db)
        {
            var users = db.GetCollection<User>(nameof(User));
            if (users == null)
            {
                _logger.Error("users does not exist");
            }

            var adminUser = await users.FindOneAsync(u => u.Name == "admin");
            var userQuick = await users.FindOneAsync(u => u.Name == "quick");
            var userBlocked = await users.FindOneAsync(u => u.Name == "blocked");

            var adminUserPassword = _passwordHasher.Hash("admin");
            adminUser ??= new User
            {
                Name = "admin",
                Password = adminUserPassword.HashedPassword,
                Nonce = adminUserPassword.Salt,
                Permissions = Permission.AllPermissions.ToList()
            };

            var userQuickPassword = _passwordHasher.Hash("quick");
            userQuick ??= new User
            {
                Name = "quick",
                Password = userQuickPassword.HashedPassword,
                Nonce = userQuickPassword.Salt,
                Permissions = Role.Editor.ToList()
            };

            var userBlockedPassword = _passwordHasher.Hash("blocked");
            userBlocked ??= new User
            {
                Name = "blocked",
                Password = userBlockedPassword.HashedPassword,
                Nonce = userBlockedPassword.Salt,
                Permissions = Role.BlockedUser.ToList()
            };

            await users.InsertAsync(new[] { adminUser, userQuick, userBlocked });
            await db.CommitAsync();

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

            var projectTemplates = db.GetCollection<ProjectTemplate>();
            await projectTemplates.InsertAsync(new[]
            {
                projectTemplateVanilla5000,
                projectTemplateVanilla7000,
                projectTemplateVanilla708711
            });
            await db.CommitAsync();

            var privateProject5000 = new Project
            {
                Name = "Private Project 1",
                Description = "Some Private Project Rex is working on, based on 5.0.0.0",
                IsPublic = false,
                Template = projectTemplateVanilla5000,
                CreatedBy = userQuick,
                CreatedAt = DateTime.UtcNow,
                AllowedUsers = new List<User> { userQuick }
            };

            var privateProject7000 = new Project
            {
                Name = "Private Project 1",
                Description = "Some Private Project Rex is working on, based on 7.0.0.0",
                IsPublic = false,
                Template = projectTemplateVanilla7000,
                CreatedBy = userQuick,
                CreatedAt = DateTime.UtcNow,
                AllowedUsers = new List<User> { userQuick, adminUser }
            };

            var publicProject = new Project
            {
                Name = "Public Project",
                Description = "Anybody can join",
                IsPublic = true,
                Template = projectTemplateVanilla708711,
                CreatedBy = adminUser,
                CreatedAt = DateTime.UtcNow,
            };

            var projects = db.GetCollection<Project>();
            await projects.InsertAsync(new []
            {
                privateProject5000,
                privateProject7000,
                publicProject
            });
            await db.CommitAsync();
        }
    }
}
