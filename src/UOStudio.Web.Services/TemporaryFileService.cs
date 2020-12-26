using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using UOStudio.Web.Data;

namespace UOStudio.Web.Services
{
    public class TemporaryFileService : ITemporaryFileService
    {
        private readonly ILogger _logger;
        private readonly IDbContextFactory<UOStudioDbContext> _contextFactory;
        private readonly ProjectOptions _projectOptions;

        public TemporaryFileService(
            ILogger logger,
            IDbContextFactory<UOStudioDbContext> contextFactory,
            IOptions<ProjectOptions> projectOptions)
        {
            _logger = logger;
            _contextFactory = contextFactory;
            _projectOptions = projectOptions.Value;
        }

        public async Task<Guid> CreateFile(Guid projectId)
        {
            return Guid.NewGuid();
        }

        public async Task<Result<Stream>> GetFileAsync(Guid projectId)
        {
            await using var context = _contextFactory.CreateDbContext();

            var file = await context.TemporaryFiles.FirstOrDefaultAsync(t => t.ProjectId == projectId);
            if (file == null)
            {
                _logger.Debug($"Requesting client.zip for project {projectId}...Not found.");
                return Result.Failure<Stream>($"File for project {projectId} not found.");
            }

            var rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);
            var tempFilesPath = Path.Combine(rootPath!, _projectOptions.TempPath);
            var filePath = Path.Combine(tempFilesPath, $"{file.FileId:N}.zip");

            if (!File.Exists(filePath))
            {
                _logger.Debug($"Requesting client.zip for project {projectId}...File does not exist.");
                return Result.Failure<Stream>("File does not exist.");
            }

            _logger.Debug($"Requesting client.zip for project {projectId}...Done.");
            return Result.Success<Stream>(File.OpenRead(filePath));
        }
    }
}
