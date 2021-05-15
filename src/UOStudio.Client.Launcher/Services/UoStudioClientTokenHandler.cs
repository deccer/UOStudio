using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace UOStudio.Client.Launcher.Services
{
    public class UoStudioClientTokenHandler : DelegatingHandler
    {
        private readonly ILogger _logger;
        private readonly ITokenClient _tokenClient;

        public UoStudioClientTokenHandler(
            ILogger logger,
            ITokenClient tokenClient)
        {
            _logger = logger.ForContext<UoStudioClientTokenHandler>();
            _tokenClient = tokenClient;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var tokenPairResult = await _tokenClient.AcquireTokenPairAsync(cancellationToken);
            if (tokenPairResult.IsFailure)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenPairResult.Value.AccessToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}