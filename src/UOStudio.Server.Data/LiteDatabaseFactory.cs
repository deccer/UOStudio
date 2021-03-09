using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB.Async;
using UOStudio.Common.Core;
using UOStudio.Server.Common;

namespace UOStudio.Server.Data
{
    /*
    public class LiteDatabaseFactory : ILiteDatabaseFactory
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly string _databaseLocation;

        public LiteDatabaseFactory(
            ServerSettings serverSettings,
            IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
            _databaseLocation = serverSettings.DatabaseLocation;
        }

        public async Task<ILiteDatabaseAsync> OpenDatabaseAsync()
        {
            var db = new LiteDatabaseAsync($"Filename={_databaseLocation};Connection=shared");
#if DEBUG
            await EnsureCreatedAsync(db);
#endif
            return db;
        }

        private async Task EnsureCreatedAsync(ILiteDatabaseAsync db)
        {
            var users = db.GetCollection<User>();
            if (await users.CountAsync() == 0)
            {
                await users.EnsureIndexAsync(user => user.Id, true);
                await users.EnsureIndexAsync(user => user.Name, true);

                await users.InsertAsync(new User(1, "admin", _passwordHasher.Hash("admin"), Role.Administrator));
                await users.InsertAsync(new User(2, "editor", _passwordHasher.Hash("editor"), Role.Editor));
                await users.InsertAsync(new User(3, "notvalid", _passwordHasher.Hash("notvalid")));

                var projectTemplates = db.GetCollection<ProjectTemplate>();
                if (await projectTemplates.CountAsync() == 0)
                {
                    await projectTemplates.EnsureIndexAsync(projectTemplate => projectTemplate.Id, true);
                    await projectTemplates.EnsureIndexAsync(projectTemplate => projectTemplate.Name, true);

                    await projectTemplates.InsertAsync(
                        new ProjectTemplate("Template1", "7.0.80.11", @"D:\UO\Vanilla\7.0.87.11\7.0.87.11-Server")
                    );
                }

                var projects = db.GetCollection<Project>();
                if (await projects.CountAsync() == 0)
                {
                    await projects.EnsureIndexAsync(project => project.Id, true);
                    await projects.EnsureIndexAsync(project => project.Name, true);

                    var projectTemplate1 = await projectTemplates.FindOneAsync(pt => pt.Id == 1);
                    await projects.InsertAsync(new Project
                    {
                        Name = "Public Project",
                        Description = "First Project",
                        Template = projectTemplate1,
                        IsPublic = true,
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = await users.FindOneAsync(u => u.Id == 1)
                    });

                    var privateProject1 = new Project
                    {
                        Name = "Private Project #1",
                        Description = "Administrator's Project",
                        Template = projectTemplate1,
                        IsPublic = false,
                        AllowedUsers = new List<User> { await users.FindByIdAsync(1) },
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = await users.FindOneAsync(u => u.Id == 1)
                    };
                    await projects.InsertAsync(privateProject1);
                }
            }
        }
    }
    */
}
