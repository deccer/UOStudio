using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Serilog;
using UOStudio.Common.Contracts;
using UOStudio.Server.Data;

namespace UOStudio.Server.Requests
{
    [UsedImplicitly]
    internal sealed class GetProjectDetailsByProjectNameRequestHandler : IRequestHandler<GetProjectDetailsByProjectNameRequest, Result<ProjectDetailDto>>
    {
        private readonly ILiteDatabaseFactory _liteDatabaseFactory;
        private readonly ILogger _logger;

        public GetProjectDetailsByProjectNameRequestHandler(
            ILogger logger,
            ILiteDatabaseFactory liteDatabaseFactory)
        {
            _logger = logger.ForContext<GetProjectDetailsByProjectNameRequestHandler>();
            _liteDatabaseFactory = liteDatabaseFactory;
        }

        public async Task<Result<ProjectDetailDto>> Handle(
            GetProjectDetailsByProjectNameRequest request,
            CancellationToken cancellationToken)
        {
            _logger.Debug("{@TypeName}", GetType().Name);
            using var db = await _liteDatabaseFactory.OpenDatabaseAsync()
                .ConfigureAwait(false);

            var user = await db.GetCollection<User>()
                .FindOneAsync(user => user.Id == request.UserId)
                .ConfigureAwait(false);
            if (user == null)
            {
                _logger.Error("User {@Id} not found", request.UserId);
                return Result.Failure<ProjectDetailDto>("User not found");
            }

            var project = await db.GetCollection<Project>()
                .FindOneAsync(project => project.Name == request.ProjectName)
                .ConfigureAwait(false);
            if (project == null)
            {
                _logger.Error("Project {@ProjectName} not found", request.ProjectName);
                return Result.Failure<ProjectDetailDto>("Project not found");
            }

            var projectDetail = new ProjectDetailDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                CreatedBy = project.CreatedBy.Name,
                Template = project.Template.Name,
                ClientVersion = project.Template.ClientVersion
            };

            _logger.Debug("ProjectDetails requested by user {@UserName} for project {@ProjectName}", user.Name, project.Name);
            return Result.Success(projectDetail);
        }
    }
}
