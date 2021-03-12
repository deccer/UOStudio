using System.IO;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Serilog;
using UOStudio.Server.Common;

namespace UOStudio.Server.Services
{
    public sealed class ProjectTemplateService : IProjectTemplateService
    {
        private readonly IGitClient _gitClient;
        private readonly ServerSettings _serverSettings;
        private readonly ILogger _logger;

        public ProjectTemplateService(
            ILogger logger,
            IGitClient gitClient,
            ServerSettings serverSettings)
        {
            _gitClient = gitClient;
            _serverSettings = serverSettings;
            _logger = logger.ForContext<ProjectTemplateService>();
        }

        public async Task<Result<string>> CreateProjectTemplate(string templateName, string clientVersion)
        {
            var templatePath = Path.Combine(_serverSettings.TemplatesDirectory, templateName);
            if (Directory.Exists(templatePath))
            {
                return Result.Failure<string>($"Template {templateName} already exists");
            }

            /*
            git://github.com/XXX/UOStudio-Templates = repository
              5.0.0.0/                              = branchName
                      map0.mul
                      statics0.mul
                      staidx0.mul
              5.0.0.0-Atlas/                        = branchName-Atlas
                            Atlas.blob
                            Atlas.json
             */

            /*
            var cloneResult = _gitClient.CloneBranch(_serverSettings.TemplateGitRepository, templateName, templatePath);
            if (cloneResult.IsSuccess)
            {
                // run texture atlas packer
            }
            // download raw assets
            // if not found download from github/gdrive and unzip map-relevant files - create atlas - place into templates
            */

            return await Task.FromResult<Result<string>>(templatePath);
        }
    }
}
