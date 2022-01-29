using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Serilog;
using UOStudio.Server.Data;
using UOStudio.Server.Services;

namespace UOStudio.Server.Domain.CreateProjectTemplate
{
    [UsedImplicitly]
    internal sealed class CreateProjectTemplateCommandHandler : IRequestHandler<CreateProjectTemplateCommand, Result<int>>
    {
        private readonly ILogger _logger;
        private readonly ILiteDbFactory _liteDbFactory;
        private readonly IProjectTemplateService _projectTemplateService;

        public CreateProjectTemplateCommandHandler(
            ILogger logger,
            ILiteDbFactory liteDbFactory,
            IProjectTemplateService projectTemplateService)
        {
            _logger = logger.ForContext<CreateProjectTemplateCommandHandler>();
            _liteDbFactory = liteDbFactory;
            _projectTemplateService = projectTemplateService;
        }

        public async Task<Result<int>> Handle(CreateProjectTemplateCommand request, CancellationToken cancellationToken)
        {
            if (!request.User.IsInRole(Permission.CanCreateProjectTemplate.Name))
            {
                return Result.Failure<int>("No permission to create project templates");
            }

            using var db = _liteDbFactory.CreateLiteDatabase();

            var projectTemplates = db.GetCollection<ProjectTemplate>();
            var projectTemplate = await projectTemplates
                .FindOneAsync(pt => pt.Name == request.Name);

            if (projectTemplate != null)
            {
                return Result.Failure<int>($"A project template name '{request.Name}' already exists");
            }

            var createProjectTemplateResult = await _projectTemplateService.CreateProjectTemplateAsync(request.Name);
            if (createProjectTemplateResult.IsFailure)
            {
                return Result.Failure<int>(createProjectTemplateResult.Error);
            }

            projectTemplate = new ProjectTemplate
            {
                Name = request.Name,
                ClientVersion = request.ClientVersion,
                Location = createProjectTemplateResult.Value
            };

            await projectTemplates.InsertAsync(projectTemplate);
            await db.CommitAsync();

            return Result.Success(projectTemplate.Id);
        }
    }
}
