using System;
using System.IO;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using Serilog;
using UOStudio.Server.Common;

namespace UOStudio.Server.Services
{
    public sealed class ProjectService : IProjectService
    {
        private readonly ILogger _logger;
        private readonly ISevenZipService _sevenZipService;
        private readonly ServerSettings _serverSettings;

        private readonly string[] _clientFiles = new[]
        {
            "hues.mul",
            "tiledata.mul"
        };

        public ProjectService(
            ILogger logger,
            ISevenZipService sevenZipService,
            IOptions<ServerSettings> serverSettings
        )
        {
            _logger = logger;
            _sevenZipService = sevenZipService;
            _serverSettings = serverSettings.Value;
        }

        public Result<bool> CreateProject(string projectTemplatePath, int projectId)
        {
            var targetProjectPath = Path.Combine(_serverSettings.ProjectsDirectory, projectId.ToString());
            var targetProjectServerPath = Path.Combine(targetProjectPath, "Server");
            var targetProjectClientPath = Path.Combine(targetProjectPath, "Client");
            Directory.CreateDirectory(targetProjectServerPath);
            Directory.CreateDirectory(targetProjectClientPath);

            var projectTemplateFiles = Directory.GetFiles(projectTemplatePath);
            foreach (var projectTemplateFile in projectTemplateFiles)
            {
                File.Copy(projectTemplateFile, Path.Combine(targetProjectPath, Path.GetFileName(projectTemplateFile)));
            }

            return Result.Success(true);
        }

        public string GetProjectPath(int projectId)
        {
            return string.IsNullOrEmpty(Path.GetDirectoryName(_serverSettings.ProjectsDirectory))
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _serverSettings.ProjectsDirectory, projectId.ToString())
                : Path.Combine(_serverSettings.ProjectsDirectory, projectId.ToString());
        }

        public Result<Guid> PrepareProjectForClientDownload(int projectId)
        {
            var projectClientPath = Path.Combine(GetProjectPath(projectId), "Client");

            var guid = Guid.NewGuid();
            var guidFileName = guid.ToString();

            var downloadsDirectory = string.IsNullOrEmpty(Path.GetDirectoryName(_serverSettings.DownloadsDirectory))
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _serverSettings.DownloadsDirectory)
                : _serverSettings.DownloadsDirectory;

            var archiveFileName = Path.Combine(downloadsDirectory, $"{guidFileName}.7z");
            _logger.Information("Project Service - Compressing {@ArchiveFileName}", archiveFileName);
            var zipResult = _sevenZipService.Zip(archiveFileName, $"{projectClientPath}\\*.*");
            if (zipResult.IsSuccess)
            {
                _logger.Information("Project Service - Compressing Done - {@Result}", guid);
                return Result.Success(guid);
            }
            else
            {
                _logger.Error("Project Service - Compressing Failed - {@Error}", zipResult.Error);
                return Result.Failure<Guid>(zipResult.Error);
            }
        }
    }
}
