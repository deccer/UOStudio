using System.IO;
using CSharpFunctionalExtensions;
using Serilog;
using UOStudio.Server.Common;

namespace UOStudio.Server.Services
{
    public sealed class ProjectService : IProjectService
    {
        private readonly ILogger _logger;
        private readonly ServerSettings _serverSettings;

        public ProjectService(ILogger logger, ServerSettings serverSettings)
        {
            _logger = logger;
            _serverSettings = serverSettings;
        }

        public Result<bool> CreateProject(string projectTemplatePath, string name)
        {
            var targetProjectPath = Path.Combine(_serverSettings.ProjectsDirectory, name);
            Directory.CreateDirectory(targetProjectPath);

            var projectTemplateFiles = Directory.GetFiles(projectTemplatePath);
            foreach (var projectTemplateFile in projectTemplateFiles)
            {
                File.Copy(projectTemplateFile, Path.Combine(targetProjectPath, Path.GetFileName(projectTemplateFile)));
            }

            return Result.Success(true);
        }
    }
}
