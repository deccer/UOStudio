using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Serilog;
using UOStudio.Common.Core.Extensions;
using UOStudio.Server.Data;
using UOStudio.Server.Services;

namespace UOStudio.Server.Domain.JoinProject
{
    [UsedImplicitly]
    internal sealed class JoinProjectCommandHandler : IRequestHandler<JoinProjectCommand, Result<JoinProjectResult>>
    {
        private readonly ILogger _logger;
        private readonly IProjectService _projectService;
        private readonly ILiteDbFactory _liteDbFactory;

        public JoinProjectCommandHandler(
            ILogger logger,
            IProjectService projectService,
            ILiteDbFactory liteDbFactory)
        {
            _logger = logger.ForContext<JoinProjectCommandHandler>();
            _projectService = projectService;
            _liteDbFactory = liteDbFactory;
        }

        public async Task<Result<JoinProjectResult>> Handle(JoinProjectCommand request, CancellationToken cancellationToken)
        {
            if (!request.User.IsInRole(Permission.CanJoinProject.Name))
            {
                return Result.Failure<JoinProjectResult>("Not allowed to join project");
            }

            var userId = request.User.GetUserId();
            using var db = _liteDbFactory.CreateLiteDatabase();

            var users = db.GetCollection<User>(nameof(User));
            var user = await users
                .FindOneAsync(u => u.Id == userId);

            if (user == null)
            {
                return Result.Failure<JoinProjectResult>("User not found");
            }

            var projects = db.GetCollection<Project>(nameof(Project));
            var project = await projects
                .Include(p => p.AllowedUsers)
                .FindOneAsync(p => p.Id == request.ProjectId);
            if (project == null)
            {
                return Result.Failure<JoinProjectResult>("Project not found");
            }

            if (!project.IsPublic && !project.AllowedUsers.Contains(user))
            {
                return Result.Failure<JoinProjectResult>($"Not allowed to join project '{project.Name}'");
            }

            var projectPath = _projectService.GetProjectPath(request.ProjectId);
            var projectClientPath = Path.Combine(projectPath, "Client");
            var atlasHashFileName = Path.Combine(projectClientPath, "Atlas.hash");
            var atlasHash = string.Empty;
            if (File.Exists(atlasHashFileName))
            {
                atlasHash = await File.ReadAllTextAsync(atlasHashFileName, cancellationToken);
            }

            if (request.AtlasHash != atlasHash)
            {
                var taskId = Guid.NewGuid();
                //_backgroundTaskQueue.QueueBackgroundWorkItem(ct => QueueWorkItem(ct, taskId, request.ProjectId));

                return Result.Success(new JoinProjectResult { NeedsPreparation = true, TaskId = taskId });
            }

            return Result.Success(new JoinProjectResult { NeedsPreparation = false, TaskId = null });
        }

        private async Task QueueWorkItem(CancellationToken cancellationToken, Guid taskId, int projectId)
        {
            // client and server differ, server needs to provide client with new atlas files for download
            await QueueBackgroundTaskAsync(taskId);

            var preparationResult = _projectService.PrepareProjectForClientDownload(projectId);

            using var db = _liteDbFactory.CreateLiteDatabase();
            var backgroundTasks = db.GetCollection<BackgroundTask>();
            var backgroundTask = await backgroundTasks
                .FindOneAsync(bt => bt.Id == taskId);

            if (preparationResult.IsSuccess)
            {
                backgroundTask.Status = BackgroundTaskStatus.Completed;
                backgroundTask.CompletedLocation = $"api/download/{preparationResult.Value}";
            }
            else
            {
                backgroundTask.Status = BackgroundTaskStatus.Failed;
                backgroundTask.ErrorMessage = preparationResult.Error;
            }

            await backgroundTasks.UpdateAsync(backgroundTask);
            await db.CommitAsync();
        }

        private async Task QueueBackgroundTaskAsync(Guid taskId)
        {
            using var db = _liteDbFactory.CreateLiteDatabase();
            var backgroundTasks = db.GetCollection<BackgroundTask>(nameof(BackgroundTask));
            var backgroundTask = new BackgroundTask
            {
                Status = BackgroundTaskStatus.Running,
                Id = taskId,
                CompletedLocation = null
            };
            await backgroundTasks.InsertAsync(backgroundTask);
            await db.CommitAsync();
        }
    }
}
