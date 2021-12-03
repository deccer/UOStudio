using System.IO;
using Microsoft.Extensions.Options;
using Serilog;

namespace UOStudio.Client.Services
{
    public sealed class ProjectService : IProjectService
    {
        private readonly ILogger _logger;
        private readonly ClientSettings _clientSettings;

        public ProjectService(
            ILogger logger,
            IOptions<ClientSettings> clientSettings)
        {
            _logger = logger;
            _clientSettings = clientSettings.Value;
        }

        public string GetProjectPath(int projectId)
        {
            return Path.Combine(_clientSettings.ProjectsDirectory, projectId.ToString());
        }
    }
}
