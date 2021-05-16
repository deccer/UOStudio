using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UOStudio.Server.Data;

namespace UOStudio.Server.Domain.CreateProjectTemplate
{
    [UsedImplicitly]
    public sealed class CreateProjectTemplateCommandHandler : IRequestHandler<CreateProjectTemplateCommand, Result<int>>
    {
        private readonly ILogger _logger;
        private readonly IDbContextFactory<UOStudioContext> _contextFactory;
        private readonly IProjectTemplateService _projectTemplateService;

        public CreateProjectTemplateCommandHandler(
            ILogger logger,
            IDbContextFactory<UOStudioContext> contextFactory)
        {
            _logger = logger.ForContext<CreateProjectTemplateCommandHandler>();
            _contextFactory = contextFactory;
        }

        public async Task<Result<int>> Handle(CreateProjectTemplateCommand request, CancellationToken cancellationToken)
        {
            if (!request.User.IsInRole(Permission.CanCreateProjectTemplate.Name))
            {
                return Result.Failure<int>("No permission to create project templates");
            }

            await using var db = _contextFactory.CreateDbContext();

            var projectTemplate = await db.ProjectTemplates
                .AsQueryable()
                .FirstOrDefaultAsync(pt => pt.Name == request.Name, cancellationToken);

            if (projectTemplate != null)
            {
                return Result.Failure<int>($"A project template name '{request.Name}' already exists");
            }

            projectTemplate = new ProjectTemplate
            {
                Name = request.Name,
                ClientVersion = request.ClientVersion,
                Location = request.Location
            };

            await db.ProjectTemplates.AddAsync(projectTemplate, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            return Result.Success(projectTemplate.Id);
        }
    }
}
