using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using LiteNetLib;
using Microsoft.Extensions.Configuration;
using Serilog;
using UOStudio.Common.Contracts;
using UOStudio.Common.Core.Extensions;

namespace UOStudio.Client.Services
{
    public class NetworkClient : INetworkClient
    {
        private readonly ILogger _logger;
        private readonly IContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenService _tokenService;
        private readonly string _apiEndpoint;
        private Profile _selectedProfile;

        public NetworkClient(
            ILogger logger,
            IConfiguration configuration,
            IContext context,
            IHttpClientFactory httpClientFactory,
            ITokenService tokenService)
        {
            _logger = logger;
            _context = context;
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
            _apiEndpoint = configuration["Api:ApiEndpoint"];
        }

        public event Action<NetPeer> Connected;
        public event Action Disconnected;
        public event Action<int, IReadOnlyCollection<ProjectDto>> LoginSucceeded;
        public event Action<string> LoginFailed;

        public event Action<IReadOnlyCollection<ProjectDto>> GetProjectsSucceeded;
        public event Action<string> GetProjectsFailed;
        public event Action<ProjectDetailDto> GetProjectDetailsSucceeded;
        public event Action<string> GetProjectDetailsFailed;

        public void Connect(Profile profile)
        {
            _selectedProfile = profile;

            var getProjectsResult = GetProjectsAfterLogin();
            if (getProjectsResult.IsSuccess)
            {
                var loginSucceeded = LoginSucceeded;
                loginSucceeded?.Invoke(_context.User.GetUserId(), getProjectsResult.Value);
            }
            else
            {
                var loginFailed = LoginFailed;
                loginFailed?.Invoke(getProjectsResult.Error);
            }
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
        }

        public void GetProjectDetailsByProjectId(int projectId)
        {
            try
            {
                var httpClientResult = _httpClientFactory.CreateWithBearerAuthentication(_tokenService, _selectedProfile);
                if (httpClientResult.IsFailure)
                {
                    var getProjectDetailsFailed = GetProjectDetailsFailed;
                    getProjectDetailsFailed?.Invoke(httpClientResult.Error);
                }

                var getProjectDetailsResponse = httpClientResult.Value.GetAsync($"{_apiEndpoint}/api/project/{projectId}").GetAwaiter().GetResult();
                if (getProjectDetailsResponse.IsSuccessStatusCode)
                {
                    var getProjectDetailsSucceeded = GetProjectDetailsSucceeded;
                    getProjectDetailsSucceeded?.Invoke(getProjectDetailsResponse.Content.ReadFromJsonAsync<ProjectDetailDto>().GetAwaiter().GetResult());
                }

                httpClientResult.Value.Dispose();
            }
            catch (Exception exception)
            {
                var getProjectDetailsFailed = GetProjectDetailsFailed;
                getProjectDetailsFailed?.Invoke(exception.Message);
            }
        }

        public void GetProjectDetailsByProjectName(string projectName)
        {
            try
            {
                var httpClientResult = _httpClientFactory.CreateWithBearerAuthentication(_tokenService, _selectedProfile);
                if (httpClientResult.IsFailure)
                {
                    var getProjectDetailsFailed = GetProjectDetailsFailed;
                    getProjectDetailsFailed?.Invoke(httpClientResult.Error);
                }

                projectName = projectName.Replace(" ", "+");
                var getProjectDetailsResponse = httpClientResult.Value.GetAsync($"{_apiEndpoint}/api/project/{projectName}").GetAwaiter().GetResult();
                if (getProjectDetailsResponse.IsSuccessStatusCode)
                {
                    var getProjectDetailsSucceeded = GetProjectDetailsSucceeded;
                    getProjectDetailsSucceeded?.Invoke(getProjectDetailsResponse.Content.ReadFromJsonAsync<ProjectDetailDto>().GetAwaiter().GetResult());
                }

                httpClientResult.Value.Dispose();
            }
            catch (Exception exception)
            {
                var getProjectDetailsFailed = GetProjectDetailsFailed;
                getProjectDetailsFailed?.Invoke(exception.Message);
            }
        }

        public void GetProjects()
        {
            try
            {
                var httpClientResult = _httpClientFactory.CreateWithBearerAuthentication(_tokenService, _selectedProfile);
                if (httpClientResult.IsFailure)
                {
                    var getProjectsFailed = GetProjectsFailed;
                    getProjectsFailed?.Invoke(httpClientResult.Error);
                }

                var getProjectResponse = httpClientResult.Value.GetAsync($"{_apiEndpoint}/api/project").GetAwaiter().GetResult();
                if (getProjectResponse.IsSuccessStatusCode)
                {
                    var getProjectsSucceeded = GetProjectsSucceeded;
                    getProjectsSucceeded?.Invoke(getProjectResponse.Content.ReadFromJsonAsync<List<ProjectDto>>().GetAwaiter().GetResult());
                }

                httpClientResult.Value.Dispose();
            }
            catch (Exception exception)
            {
                var getProjectsFailed = GetProjectsFailed;
                getProjectsFailed?.Invoke(exception.Message);
            }
        }

        private Result<IReadOnlyCollection<ProjectDto>> GetProjectsAfterLogin()
        {
            try
            {
                var httpClientResult = _httpClientFactory.CreateWithBearerAuthentication(_tokenService, _selectedProfile);
                if (httpClientResult.IsFailure)
                {
                    return Result.Failure<IReadOnlyCollection<ProjectDto>>(httpClientResult.Error);
                }

                var getProjectResponse = httpClientResult.Value.GetAsync($"{_apiEndpoint}/api/project").GetAwaiter().GetResult();
                httpClientResult.Value.Dispose();

                return getProjectResponse.IsSuccessStatusCode
                    ? Result.Success<IReadOnlyCollection<ProjectDto>>(getProjectResponse.Content.ReadFromJsonAsync<List<ProjectDto>>().GetAwaiter().GetResult())
                    : Result.Failure<IReadOnlyCollection<ProjectDto>>(getProjectResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            }
            catch (Exception exception)
            {
                return Result.Failure<IReadOnlyCollection<ProjectDto>>(exception.Message);
            }
        }
    }
}
