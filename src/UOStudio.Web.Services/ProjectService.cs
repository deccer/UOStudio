using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using UOStudio.Core.Extensions;
using UOStudio.Shared.Network;
using UOStudio.Web.Contracts;
using UOStudio.Web.Data;
using UOStudio.Web.Data.Entities;
using UOStudio.Web.Extensions;

namespace UOStudio.Web.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ProjectOptions _projectOptions;
        private readonly ILogger _logger;
        private readonly IDbContextFactory<UOStudioDbContext> _contextFactory;
        private readonly string _rootPath;

        public ProjectService(
            ILogger logger,
            IOptions<ProjectOptions> options,
            IDbContextFactory<UOStudioDbContext> contextFactory)
        {
            _logger = logger;
            _projectOptions = options.Value;
            _contextFactory = contextFactory;
            _rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);
        }

        public async Task<IEnumerable<ProjectDto>> GetAllAsync()
        {
            await using var context = _contextFactory.CreateDbContext();

            var projects = await context.Projects
                .ToListAsync()
                .ConfigureAwait(false);
            return projects.ToDto();
        }

        public async Task<Result<ProjectDto>> GetProjectAsync(Guid projectId, Guid userId)
        {
            await using var context = _contextFactory.CreateDbContext();

            var user = await context.Users
                .FindAsync(userId)
                .ConfigureAwait(false);
            if (user == null)
            {
                return Result.Failure<ProjectDto>("User not found.");
            }

            if (user.IsBlocked())
            {
                return Result.Failure<ProjectDto>("User has no privileges.");
            }

            var project = await context.Projects
                .FindAsync(projectId)
                .ConfigureAwait(false);
            return Result.Success(project.ToDto());
        }

        public async Task<Result<Guid>> CreateProjectAsync(string projectName, string projectDescription, string projectClientVersion, Guid userId)
        {
            await using var context = _contextFactory.CreateDbContext();

            var user = await context.Users
                .FindAsync(userId)
                .ConfigureAwait(false);
            if (user == null)
            {
                return Result.Failure<Guid>("User not found.");
            }

            if (user.IsBlocked())
            {
                return Result.Failure<Guid>("User has no privileges.");
            }

            var project = await context.Projects
                .FirstOrDefaultAsync(p => p.Name == projectName)
                .ConfigureAwait(false);
            if (project != null)
            {
                var message = $"Project {projectName} already exist.";
                _logger.Error(message);
                return Result.Failure<Guid>(message);
            }

            var projectId = Guid.NewGuid();
            var projectPath = Path.Combine(_rootPath!, _projectOptions.ProjectsPath, projectId.ToString("N"));
            Directory.CreateDirectory(projectPath);
            var templateFilePath = Path.Combine(_rootPath!, _projectOptions.TemplatePath, projectClientVersion);
            var serverTemplateFilePath = Path.Combine(templateFilePath, "server.zip");
            var serverProjectPath = Path.Combine(projectPath, "server");
            Directory.CreateDirectory(serverProjectPath);
            var clientTemplateFilePath = Path.Combine(templateFilePath, "client.zip");
            var clientProjectPath = Path.Combine(projectPath, "client");
            Directory.CreateDirectory(clientProjectPath);

            _logger.Debug($"Preparing Project {projectName}...");
            ZipFile.ExtractToDirectory(serverTemplateFilePath, serverProjectPath);
            ZipFile.ExtractToDirectory(clientTemplateFilePath, clientProjectPath);
            _logger.Debug($"Preparing Project {projectName}...Done.");

            project = new Project
            {
                Id = projectId,
                Name = projectName,
                Description = projectDescription,
                ClientVersion = projectClientVersion
            };

            await context.Projects
                .AddAsync(project)
                .ConfigureAwait(false);
            await context
                .SaveChangesAsync()
                .ConfigureAwait(false);

            return Result.Success(project.Id);
        }

        public async Task<Result> DeleteProjectAsync(Guid projectId, Guid userId)
        {
            await using var context = _contextFactory
                .CreateDbContext();

            var projectToDelete = await context.Projects
                .FindAsync(projectId)
                .ConfigureAwait(false);
            if (projectToDelete == null)
            {
                return Result.Failure($"Project {projectId} not found.");
            }

            // find existing temporary files and get rid of those
            var tempPath = Path.Combine(_rootPath!, _projectOptions.TempPath);
            var filesToDelete = context.TemporaryFiles.Where(tf => tf.ProjectId == projectId);
            foreach (var temporaryFile in filesToDelete)
            {
                var tempFilePath = Path.Combine(tempPath, $"{temporaryFile.FileId:N}.zip");
                File.Delete(tempFilePath);
            }

            context.TemporaryFiles.RemoveRange(filesToDelete);
            await context
                .SaveChangesAsync()
                .ConfigureAwait(false);

            // delete project files
            var projectPath = Path.Combine(_rootPath!, _projectOptions.ProjectsPath, projectId.ToString("N"));
            var projectFiles = Directory.GetFiles(projectPath, "*.*");
            foreach (var projectFile in projectFiles)
            {
                File.Delete(projectFile);
            }
            Directory.Delete(projectPath);

            context.Projects.Remove(projectToDelete);

            return Result.Success();
        }

        public async Task<Result<JoinResult>> JoinProjectAsync(Guid projectId, Guid userId, byte[] projectClientHash)
        {
            await using var context = _contextFactory
                .CreateDbContext();

            var project = await context.Projects
                .FindAsync(projectId)
                .ConfigureAwait(false);
            if (project == null)
            {
                return Result.Failure<JoinResult>($"Project {projectId} not found.");
            }

            var clientProjectPath = Path.Combine(_rootPath!, _projectOptions.ProjectsPath, projectId.ToString("N"), "client");
            var clientProjectPathInfo = new DirectoryInfo(clientProjectPath);

            using var hashAlgorithm = MD5.Create();
            var projectServerHash = await hashAlgorithm
                .ComputeHashAsync(clientProjectPathInfo.EnumerateFiles("*.mul", SearchOption.TopDirectoryOnly), false)
                .ConfigureAwait(false);
            if (projectClientHash.SequenceEqual(projectServerHash))
            {
                return Result.Success(JoinResult.ClientUpToDate);
            }

            var fileId = Guid.NewGuid();
            var tempPath = Path.Combine(_rootPath!, _projectOptions.TempPath);
            Directory.CreateDirectory(tempPath);
            var tempFilePath = Path.Combine(tempPath, $"{fileId:N}.zip");

            ZipFile.CreateFromDirectory(clientProjectPath, tempFilePath);

            var temporaryFile = new TemporaryFile
            {
                ProjectId = projectId,
                FileId = fileId
            };
            await context.TemporaryFiles
                .AddAsync(temporaryFile)
                .ConfigureAwait(false);
            await context
                .SaveChangesAsync()
                .ConfigureAwait(false);
            return Result.Success(JoinResult.ClientRequiresUpdate);
        }
    }
}
