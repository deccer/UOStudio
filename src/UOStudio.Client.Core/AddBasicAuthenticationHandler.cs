using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace UOStudio.Client.Core
{
    public class AddBasicAuthenticationHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authHeader = await File.ReadAllTextAsync(Path.Combine(Path.GetTempPath(), "uostudio.uostudio"), cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
