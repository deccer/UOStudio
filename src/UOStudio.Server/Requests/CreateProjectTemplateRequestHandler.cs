using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Serilog;
using UOStudio.Server.Data;
using UOStudio.Server.Services;

namespace UOStudio.Server.Requests
{
    [UsedImplicitly]
    internal sealed class CreateProjectTemplateRequestHandler : IRequestHandler<CreateProjectTemplateRequest, Result<int>>
    {
        private readonly ILogger _logger;
        private readonly ILiteDatabaseFactory _liteDatabaseFactory;
        private readonly IProjectTemplateService _projectTemplateService;

        public CreateProjectTemplateRequestHandler(
            ILogger logger,
            ILiteDatabaseFactory liteDatabaseFactory,
            IProjectTemplateService projectTemplateService)
        {
            _logger = logger.ForContext<CreateProjectTemplateRequestHandler>();
            _liteDatabaseFactory = liteDatabaseFactory;
            _projectTemplateService = projectTemplateService;
        }

        public async Task<Result<int>> Handle(CreateProjectTemplateRequest request, CancellationToken cancellationToken)
        {
            _logger.Debug("{@TypeName}", GetType().Name);
            using var db = await _liteDatabaseFactory.OpenDatabaseAsync()
                .ConfigureAwait(false);

            var user = await db.GetCollection<User>()
                .FindOneAsync(user => user.Id == request.UserId)
                .ConfigureAwait(false);
            if (user == null)
            {
                _logger.Error("User with id {@Id} not found", request.UserId);
                return Result.Failure<int>("Unable to find user");
            }

            if (!user.HasPermission(Permission.CanCreateProjectTemplate))
            {
                _logger.Error("User {@UserName} not allowed to create project template", user.Name);
                return Result.Failure<int>("Not allowed to create project template");
            }

            var projectTemplate = await db.GetCollection<ProjectTemplate>()
                .FindOneAsync(pt => pt.Name == request.TemplateName)
                .ConfigureAwait(false);
            if (projectTemplate != null)
            {
                _logger.Error("Template {@TemplateName} already exists", projectTemplate.Name);
                return Result.Failure<int>("Template already exists");
            }

            // find template pack per request.ClientVersion
            var result = await _projectTemplateService.CreateProjectTemplate(request.TemplateName, request.ClientVersion);
            if (result.IsFailure)
            {
                return Result.Failure<int>(result.Error);
            }

            projectTemplate = new ProjectTemplate(request.TemplateName, request.ClientVersion, result.Value);

            await db.GetCollection<ProjectTemplate>().InsertAsync(projectTemplate)
                .ConfigureAwait(false);
            await db.CommitAsync()
                .ConfigureAwait(false);

            return Result.Success(projectTemplate.Id);
        }
    }
}
