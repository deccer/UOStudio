using System.IO;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Serilog;
using UOStudio.Server.Common;

namespace UOStudio.Server.Services
{
    public sealed class ProjectTemplateService : IProjectTemplateService
    {
        private readonly ILogger _logger;
        private readonly ServerSettings _serverSettings;

        public ProjectTemplateService(
            ILogger logger,
            ServerSettings serverSettings)
        {
            _logger = logger.ForContext<ProjectTemplateService>();
            _serverSettings = serverSettings;
        }

        public async Task<Result<string>> CreateProjectTemplateAsync(string templateName)
        {
            var templatePath = Path.Combine(_serverSettings.TemplatesDirectory, templateName);
            if (Directory.Exists(templatePath))
            {
                return Result.Failure<string>($"Template {templateName} already exists");
            }

            Directory.CreateDirectory(templatePath);
            Directory.CreateDirectory(Path.Combine(templatePath, "Client"));
            Directory.CreateDirectory(Path.Combine(templatePath, "Server"));

            return await Task.FromResult<Result<string>>(templatePath);
        }
    }
}
