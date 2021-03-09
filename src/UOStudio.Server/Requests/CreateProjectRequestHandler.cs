using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Serilog;
using UOStudio.Server.Data;

namespace UOStudio.Server.Requests
{
    [UsedImplicitly]
    internal sealed class CreateProjectRequestHandler : IRequestHandler<CreateProjectRequest, Result<int>>
    {
        private readonly ILogger _logger;
        private readonly ILiteDatabaseFactory _liteDatabaseFactory;

        public CreateProjectRequestHandler(
            ILogger logger,
            ILiteDatabaseFactory liteDatabaseFactory)
        {
            _logger = logger.ForContext<CreateProjectRequestHandler>();
            _liteDatabaseFactory = liteDatabaseFactory;
        }

        public async Task<Result<int>> Handle(CreateProjectRequest request, CancellationToken cancellationToken)
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

            if (user.HasPermission(Permission.CanCreateProject))
            {
                _logger.Error("User {@UserName} has no permission to create project {@ProjectName}", user.Name, request.Name);
                return Result.Failure<int>("No Permission to create a project");
            }

            var template = await db.GetCollection<ProjectTemplate>()
                .FindOneAsync(pt => pt.Id == request.TemplateId)
                .ConfigureAwait(false);
            if (template == null)
            {
                _logger.Error("Template {@TemplateId} was not found", request.TemplateId);
                return Result.Failure<int>("Unable to find template");
            }

            var project = new Project
            {
                Name = request.Name,
                Description = request.Description,
                Template = template,
                IsPublic = request.IsPublic,
                CreatedAt = DateTimeOffset.Now,
                CreatedBy = user
            };

            await db.GetCollection<Project>().InsertAsync(project).ConfigureAwait(false);
            await db.CommitAsync().ConfigureAwait(false);

            return Result.Success(project.Id);
        }
    }
}
