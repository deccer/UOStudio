using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using UOStudio.Common.Contracts;
using UOStudio.Server.Data.Extensions;

namespace UOStudio.Server.Data
{
    internal sealed class UserRepository : IUserRepository
    {
        private readonly ILiteDbFactory _liteDbFactory;

        public UserRepository(ILiteDbFactory liteDbFactory)
        {
            _liteDbFactory = liteDbFactory;
        }

        public async Task<Result<UserDto>> GetUserAsync(string userName)
        {
            using var db = _liteDbFactory.CreateLiteDatabase();

            var users = db.GetCollection<User>(nameof(User));
            var user = await users
                .Include(user => user.Permissions)
                .FindOneAsync(user => user.Name == userName);

            return user == null
                ? Result.Failure<UserDto>("Invalid username")
                : Result.Success(user.ToDto());
        }

        public async Task<Result<UserDto>> GetUserByRefreshTokenAsync(string refreshToken)
        {
            using var db = _liteDbFactory.CreateLiteDatabase();

            var users = db.GetCollection<User>(nameof(User));
            var user = await users
                .FindOneAsync(user => user.RefreshToken == refreshToken);

            return user == null
                ? Result.Failure<UserDto>("Invalid refresh token")
                : Result.Success(user.ToDto());
        }

        public async Task<Result> UpdateRefreshTokenAsync(int userId, string refreshToken)
        {
            using var db = _liteDbFactory.CreateLiteDatabase();

            var users = db.GetCollection<User>(nameof(User));
            var user = await users
                .FindOneAsync(user => user.Id == userId);

            if (user == null)
            {
                return Result.Failure("User not found");
            }
            user.RefreshToken = refreshToken;

            await users.UpdateAsync(user);
            await db.CommitAsync();

            return Result.Success();
        }
    }
}
