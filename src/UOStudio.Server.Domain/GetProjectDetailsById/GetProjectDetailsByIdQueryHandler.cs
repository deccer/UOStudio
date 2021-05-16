using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UOStudio.Common.Contracts;
using UOStudio.Server.Data;
using UOStudio.Server.Domain.Mappers;

namespace UOStudio.Server.Domain.GetProjectDetailsById
{
    [UsedImplicitly]
    internal class GetProjectDetailsByIdQueryHandler : IRequestHandler<GetProjectDetailsByIdQuery, Result<ProjectDetailDto>>
    {
        private readonly IDbContextFactory<UOStudioContext> _contextFactory;

        public GetProjectDetailsByIdQueryHandler(IDbContextFactory<UOStudioContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Result<ProjectDetailDto>> Handle(
            GetProjectDetailsByIdQuery query,
            CancellationToken cancellationToken)
        {
            await using var db = _contextFactory.CreateDbContext();
            var project = await db.Projects
                .AsNoTracking()
                .Include(p => p.Template)
                .Include(p => p.AllowedUsers)
                .Include(p => p.CreatedBy)
                .FirstOrDefaultAsync(p => p.Id == query.ProjectId, cancellationToken)
                .ConfigureAwait(false);

            return project == null
                ? Result.Failure<ProjectDetailDto>($"Project with id {query.ProjectId} not found")
                : Result.Success(project.ToDetailDto());
        }
    }
}
