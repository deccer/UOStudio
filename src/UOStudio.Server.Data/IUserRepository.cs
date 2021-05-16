using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using UOStudio.Common.Contracts;

namespace UOStudio.Server.Data
{
    public interface IUserRepository
    {
        Task<Result<UserDto>> GetUserAsync(string userName);

        Task<Result<UserDto>> GetUserByRefreshTokenAsync(string refreshToken);

        Task<Result> UpdateRefreshTokenAsync(int userId, string refreshToken);
    }
}
