using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Serilog;
using UOStudio.Common.Contracts;
using ProjectDto = UOStudio.Client.Launcher.Contracts.ProjectDto;

namespace UOStudio.Client.Launcher.Services
{
    public class ClientStarterService : IClientStarterService
    {
        private readonly ILogger _logger;
        private readonly IUserContext _userContext;
        private readonly ClientSettings _clientSettings;

        public ClientStarterService(
            ILogger logger,
            IUserContext userContext,
            ClientSettings clientSettings)
        {
            _logger = logger;
            _userContext = userContext;
            _clientSettings = clientSettings;
        }

        public Result StartClientAsync(ProjectDto project)
        {
            var clientStartParameters = CreateClientStartParameters(project);
            var startParameter = JsonSerializer.Serialize(clientStartParameters);
            var startParameterEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(startParameter));

            var clientPath = Path.Combine(_clientSettings.ClientDirectory, "UOStudio.Client.exe");
            if (!File.Exists(clientPath))
            {
                return Result.Failure($"Client path {clientPath} does not exist");
            }
            var process = Process.Start(clientPath, startParameterEncoded);

            return process?.Handle != IntPtr.Zero
                ? Result.Success()
                : Result.Failure("Unable to start UOStudio.Client");
        }

        private ClientStartParameters CreateClientStartParameters(ProjectDto project)
        {
            return new ClientStartParameters
            {
                ConnectionTicket = _userContext.ConnectionTicket,
                UserName = _userContext.UserCredentials.UserName,
                ServerName = _userContext.ServerName,
                ServerPort = _userContext.ServerPort,
                ProjectId = project.Id,
                ProjectName = project.Name
            };
        }
    }
}
