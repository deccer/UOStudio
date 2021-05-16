using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using UOStudio.Common.Contracts;

namespace UOStudio.Server.Api.Services
{
    public interface ITokenService
    {
        Task<Result<TokenPair>> GetTokenAsync(string userName);

        Task<Result<TokenPair>> GetTokenFromRefreshTokenAsync(string refreshToken);
    }
}
