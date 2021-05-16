using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using UOStudio.Common.Contracts;

namespace UOStudio.Server.Api.Services
{
    public interface IAuthenticationService
    {
        Task<Result<TokenTriplet>> AuthenticateAsync(UserCredentials userCredentials);

        Task<Result<TokenPair>> RefreshTokenAsync(string refreshToken);
    }
}
