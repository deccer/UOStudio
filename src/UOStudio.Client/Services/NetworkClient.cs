using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Threading;
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
        public event Action<ProjectDetailDto> GetProjectDetailsSucceeded;
        public event Action<string> GetProjectDetailsFailed;

        public void Connect(Profile profile)
        {
            _selectedProfile = profile;

            var getProjectsResult = GetProjects();
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
            throw new NotImplementedException();
        }

        public void GetProjectDetailsByProjectName(string projectName)
        {
            throw new NotImplementedException();
        }

        public Result<IReadOnlyCollection<ProjectDto>> GetProjects()
        {
            try
            {
                var retrieveTokenResult = _tokenService
                    .RetrieveTokenAsync(_selectedProfile.UserName, _selectedProfile.Password)
                    .GetAwaiter()
                    .GetResult();
                if (retrieveTokenResult.IsFailure)
                {
                    return Result.Failure<IReadOnlyCollection<ProjectDto>>(retrieveTokenResult.Error);
                }

                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(MediaTypeNames.Application.Json));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", retrieveTokenResult.Value);

                var getProjectResponse = httpClient.GetAsync($"{_apiEndpoint}/api/project").GetAwaiter().GetResult();
                if (getProjectResponse.IsSuccessStatusCode)
                {
                    return Result.Success<IReadOnlyCollection<ProjectDto>>(getProjectResponse.Content.ReadFromJsonAsync<List<ProjectDto>>().GetAwaiter().GetResult());
                }

                throw new Exception();
            }
            catch (Exception exception)
            {
                return Result.Failure<IReadOnlyCollection<ProjectDto>>(exception.ToString());
            }
        }
    }
}
