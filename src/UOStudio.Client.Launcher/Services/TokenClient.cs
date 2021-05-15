using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Serilog;
using UOStudio.Client.Launcher.Contracts;

namespace UOStudio.Client.Launcher.Services
{
    internal class TokenClient : ITokenClient
    {
        private readonly ILogger _logger;
        private readonly IUserContext _userContext;
        private readonly HttpClient _httpClient;

        public TokenClient(
            ILogger logger,
            IUserContext userContext,
            HttpClient httpClient)
        {
            _logger = logger.ForContext<TokenClient>();
            _userContext = userContext;
            _httpClient = httpClient;
        }

        public async Task<Result<TokenPair>> AcquireTokenPairAsync(CancellationToken cancellationToken = default)
        {
            var loginResponse = await _httpClient.PostAsJsonAsync($"{_userContext.AuthBaseUri}api/auth", _userContext.UserCredentials, cancellationToken);
            if (loginResponse.IsSuccessStatusCode)
            {
                var tokenPair = await loginResponse.Content.ReadFromJsonAsync<TokenPair>(cancellationToken: cancellationToken);
                if (tokenPair != null)
                {
                    _userContext.ConnectionTicket = loginResponse.Headers.GetValues("X-Ticket").FirstOrDefault();
                    return Result.Success(tokenPair);
                }
                
                return Result.Failure<TokenPair>("Unable to login");
            }

            return Result.Failure<TokenPair>("Unable to login");
        }
    }
}