using System.Net.Http.Headers;
using System.Net.Mime;
using CSharpFunctionalExtensions;
using UOStudio.Client;
using UOStudio.Client.Services;

namespace System.Net.Http
{
    public static class HttpClientFactoryExtensions
    {
        public static Result<HttpClient> CreateWithBearerAuthentication(this IHttpClientFactory httpClientFactory, ITokenService tokenService, Profile profile)
        {
            var retrieveTokenResult = tokenService
                .RetrieveTokenAsync(profile.UserName, profile.Password)
                .GetAwaiter()
                .GetResult();
            if (retrieveTokenResult.IsFailure)
            {
                return Result.Failure<HttpClient>(retrieveTokenResult.Error);
            }

            var httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(MediaTypeNames.Application.Json));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", retrieveTokenResult.Value);

            return Result.Success(httpClient);
        }
    }
}
