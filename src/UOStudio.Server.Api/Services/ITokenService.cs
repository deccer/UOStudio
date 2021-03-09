using System.Threading.Tasks;
using UOStudio.Common.Contracts;
using UOStudio.Server.Data;

namespace UOStudio.Server.Api.Services
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(User user, string refreshToken);

        string GenerateRefreshTokenAsync();
    }
}
