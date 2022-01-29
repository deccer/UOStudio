using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using UOStudio.Common.Contracts;
using UOStudio.Common.Core.Extensions;
using UOStudio.Server.Data;

namespace UOStudio.Server.Domain.GetProjects
{
    [UsedImplicitly]
    internal sealed class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, Result<IList<ProjectDto>>>
    {
        private readonly ILiteDbFactory _liteDbFactory;

        public GetProjectsQueryHandler(ILiteDbFactory liteDbFactory)
        {
            _liteDbFactory = liteDbFactory;
        }

        public async Task<Result<IList<ProjectDto>>> Handle(
            GetProjectsQuery request,
            CancellationToken cancellationToken)
        {
            using var db = _liteDbFactory.CreateLiteDatabase();

            var userId = request.User.GetUserId();
            var users = db.GetCollection<User>(nameof(User));
            var user = await users
                .Include(u => u.Permissions)
                .FindOneAsync(u => u.Id == userId);

            var projects = db.GetCollection<Project>(nameof(Project));
            var publicProjects = (await projects
                    .Include(p => p.CreatedBy)
                    .FindAsync(p => p.IsPublic))
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();

            var nonPublicProjects = await projects
                .Include(p => p.CreatedBy)
                .Include(p => p.AllowedUsers)
                .FindAsync(p => !p.IsPublic);

            if (!Role.IsAdministrator(user))
            {
                nonPublicProjects = nonPublicProjects.Where(project => project.AllowedUsers.Contains(user));
            }

            var visibleProjects = nonPublicProjects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            }).AsEnumerable().Concat(publicProjects);

            return Result.Success<IList<ProjectDto>>(visibleProjects.ToList());
        }
    }
}
