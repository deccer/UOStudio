using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        private readonly IDbContextFactory<UOStudioContext> _contextFactory;

        public JoinProjectCommandHandler(
            ILogger logger,
            IProjectService projectService,
            IDbContextFactory<UOStudioContext> contextFactory)
        {
            _logger = logger.ForContext<JoinProjectCommandHandler>();
            _projectService = projectService;
            _contextFactory = contextFactory;
        }

        public async Task<Result<JoinProjectResult>> Handle(JoinProjectCommand request, CancellationToken cancellationToken)
        {
            if (!request.User.IsInRole(Permission.CanJoinProject.Name))
            {
                return Result.Failure<JoinProjectResult>("Not allowed to join project");
            }

            var userId = request.User.GetUserId();
            await using var db = _contextFactory.CreateDbContext();
            var user = await db.Users
                .FindAsync(new object[] { userId }, cancellationToken);
            if (user == null)
            {
                return Result.Failure<JoinProjectResult>("User not found");
            }

            var project = await db.Projects
                .Include(p => p.AllowedUsers)
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);
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
            var db = _contextFactory.CreateDbContext();
            var backgroundTask = new BackgroundTask
            {
                Status = BackgroundTaskStatus.Running,
                Id = taskId,
                CompletedLocation = null
            };
            await db.BackgroundTasks.AddAsync(backgroundTask, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
            await db.DisposeAsync();

            var preparationResult = _projectService.PrepareProjectForClientDownload(projectId);

            db = _contextFactory.CreateDbContext();
            backgroundTask = await db.BackgroundTasks.FirstOrDefaultAsync(bt => bt.Id == taskId, cancellationToken);
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

            db.BackgroundTasks.Update(backgroundTask);
            await db.SaveChangesAsync(cancellationToken);

            await db.DisposeAsync();
        }
    }
}
