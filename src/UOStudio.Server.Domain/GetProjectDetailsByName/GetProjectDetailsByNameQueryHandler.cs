using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using UOStudio.Common.Contracts;
using UOStudio.Server.Data;
using UOStudio.Server.Domain.Mappers;

namespace UOStudio.Server.Domain.GetProjectDetailsByName
{
    [UsedImplicitly]
    internal sealed class GetProjectDetailsByNameQueryHandler : IRequestHandler<GetProjectDetailsByNameQuery, Result<ProjectDetailDto>>
    {
        private readonly ILiteDbFactory _liteDbFactory;

        public GetProjectDetailsByNameQueryHandler(ILiteDbFactory liteDbFactory)
        {
            _liteDbFactory = liteDbFactory;
        }

        public async Task<Result<ProjectDetailDto>> Handle(
            GetProjectDetailsByNameQuery query,
            CancellationToken cancellationToken)
        {
            using var db = _liteDbFactory.CreateLiteDatabase();
            var projects = db.GetCollection<Project>();
            var project = await projects
                .Include(p => p.Template)
                .Include(p => p.AllowedUsers)
                .Include(p => p.CreatedBy)
                .FindOneAsync(p => p.Name == query.ProjectName);

            return project == null
                ? Result.Failure<ProjectDetailDto>($"Project {query.ProjectName} not found")
                : Result.Success(project.ToDetailDto());
        }
    }
}
