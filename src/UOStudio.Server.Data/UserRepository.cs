using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using UOStudio.Common.Contracts;
using UOStudio.Server.Data.Extensions;

namespace UOStudio.Server.Data
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<UOStudioContext> _contextFactory;

        public UserRepository(IDbContextFactory<UOStudioContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Result<UserDto>> GetUserAsync(string userName)
        {
            await using var db = _contextFactory.CreateDbContext();

            var user = await db.Users
                .AsNoTracking()
                .Include(u => u.Permissions)
                .FirstOrDefaultAsync(u => u.Name == userName);
            return user == null
                ? Result.Failure<UserDto>("Invalid username")
                : Result.Success(user.ToDto());
        }

        public async Task<Result<UserDto>> GetUserByRefreshTokenAsync(string refreshToken)
        {
            await using var db = _contextFactory.CreateDbContext();

            var user = await db.Users
                .AsNoTracking()
                .Include(u => u.Permissions)
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            return user == null
                ? Result.Failure<UserDto>("Invalid refresh token")
                : Result.Success(user.ToDto());
        }

        public async Task<Result> UpdateRefreshTokenAsync(int userId, string refreshToken)
        {
            await using var db = _contextFactory.CreateDbContext();

            var user = await db.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return Result.Failure("User not found");
            }
            user.RefreshToken = refreshToken;

            db.Users.Update(user);
            await db.SaveChangesAsync();

            return Result.Success();
        }
    }
}
