using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using UOStudio.Common.Contracts;
using UOStudio.Server.Data;
using UOStudio.Server.Domain.Mappers;

namespace UOStudio.Server.Domain.GetProjectDetailsById
{
    [UsedImplicitly]
    internal class GetProjectDetailsByIdQueryHandler : IRequestHandler<GetProjectDetailsByIdQuery, Result<ProjectDetailDto>>
    {
        private readonly ILiteDbFactory _liteDbFactory;

        public GetProjectDetailsByIdQueryHandler(ILiteDbFactory liteDbFactory)
        {
            _liteDbFactory = liteDbFactory;
        }

        public async Task<Result<ProjectDetailDto>> Handle(
            GetProjectDetailsByIdQuery query,
            CancellationToken cancellationToken)
        {
            using var db = _liteDbFactory.CreateLiteDatabase();
            var projects = db.GetCollection<Project>();
            var project = await projects
                .Include(p => p.Template)
                .Include(p => p.AllowedUsers)
                .Include(p => p.CreatedBy)
                .FindOneAsync(p => p.Id == query.ProjectId);

            return project == null
                ? Result.Failure<ProjectDetailDto>($"Project with id {query.ProjectId} not found")
                : Result.Success(project.ToDetailDto());
        }
    }
}
