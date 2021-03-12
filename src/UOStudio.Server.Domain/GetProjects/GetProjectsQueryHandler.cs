using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UOStudio.Common.Contracts;
using UOStudio.Common.Core.Extensions;
using UOStudio.Server.Data;

namespace UOStudio.Server.Domain.GetProjects
{
    [UsedImplicitly]
    public sealed class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, Result<IList<ProjectDto>>>
    {
        private readonly IDbContextFactory<UOStudioContext> _contextFactory;

        public GetProjectsQueryHandler(IDbContextFactory<UOStudioContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Result<IList<ProjectDto>>> Handle(
            GetProjectsQuery request,
            CancellationToken cancellationToken)
        {
            await using var db = _contextFactory.CreateDbContext();

            var userId = request.User.GetUserId();
            var user = await db.Users
                .Include(u => u.Permissions)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
                .ConfigureAwait(false);

            var publicProjects = db.Projects
                .Include(p => p.CreatedBy)
                .Where(p => p.IsPublic)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList();

            var projects = db.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.AllowedUsers)
                .Where(p => !p.IsPublic);

            if (!Role.IsAdministrator(user))
            {
                projects = projects.Where(project => project.AllowedUsers.Contains(user));
            }

            var visibleProjects = projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList().Concat(publicProjects);

            return Result.Success<IList<ProjectDto>>(visibleProjects.ToList());
        }
    }
}
