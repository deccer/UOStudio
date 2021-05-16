using System.IO;
using Serilog;

namespace UOStudio.Client
{
    public sealed class ProjectService : IProjectService
    {
        private readonly ILogger _logger;
        private readonly ClientSettings _clientSettings;

        public ProjectService(ILogger logger, ClientSettings clientSettings)
        {
            _logger = logger;
            _clientSettings = clientSettings;
        }

        public string GetProjectPath(int projectId)
        {
            return Path.Combine(_clientSettings.ProjectsDirectory, projectId.ToString());
        }
    }
}
