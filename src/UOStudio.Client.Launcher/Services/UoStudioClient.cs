using System.Collections.Immutable;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Serilog;
using UOStudio.Client.Launcher.Contracts;

namespace UOStudio.Client.Launcher.Services
{
    public class UoStudioClient : IUoStudioClient
    {
        private readonly ILogger _logger;
        private readonly ClientSettings _clientSettings;
        private readonly IUserContext _userContext;
        private readonly HttpClient _httpClient;

        public UoStudioClient(
            ILogger logger,
            ClientSettings clientSettings,
            IUserContext userContext,
            HttpClient httpClient)
        {
            _logger = logger;
            _clientSettings = clientSettings;
            _userContext = userContext;
            _httpClient = httpClient;
        }

        public async Task<Result<IImmutableList<ProjectDto>>> GetProjectsAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"{_userContext.ApiBaseUri}api/project", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Failure<IImmutableList<ProjectDto>>("Unable to retrieve projects");
            }

            var projects = await response.Content.ReadFromJsonAsync<IImmutableList<ProjectDto>>(cancellationToken: cancellationToken);
            return Result.Success(projects);
        }
    }
}