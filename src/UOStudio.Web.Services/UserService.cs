using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UOStudio.Core;
using UOStudio.Web.Contracts;
using UOStudio.Web.Data;
using UOStudio.Web.Data.Entities;
using UOStudio.Web.Extensions;

namespace UOStudio.Web.Services
{
    public class UserService : IUserService
    {
        private readonly IDbContextFactory<UOStudioDbContext> _contextFactory;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPasswordVerifier _passwordVerifier;

        public UserService(
            IDbContextFactory<UOStudioDbContext> contextFactory,
            IPasswordHasher passwordHasher,
            IPasswordVerifier passwordVerifier)
        {
            _contextFactory = contextFactory;
            _passwordHasher = passwordHasher;
            _passwordVerifier = passwordVerifier;
        }

        public async Task<UserDto> AuthenticateAsync(string userName, string password)
        {
            await using var context = _contextFactory.CreateDbContext();
            var user = await context.Users.SingleOrDefaultAsync(x => x.UserName == userName);
            if (user == null)
            {
                return null;
            }

            if (user.IsBlocked())
            {
                return null;
            }

            return _passwordVerifier.Verify(password, user.PasswordHash)
                ? user.ToDto()
                : null;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            var users = context.Users.ToDto();
            return users;
        }

        public async Task<UserDto> GetUserAsync(Guid userId)
        {
            await using var context = _contextFactory.CreateDbContext();
            var user = await context.Users.FindAsync(userId);

            return user.ToDto();
        }

        public async Task<UserDto> GetUserByNameAndVerifyPasswordAsync(string userName, string password)
        {
            await using var context = _contextFactory.CreateDbContext();
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            return _passwordVerifier.Verify(password, user.PasswordHash)
                ? user.ToDto()
                : null;
        }

        public async Task<Guid> CreateUserAsync(string username, string password, string displayName, Permissions permissions)
        {
            await using var context = _contextFactory.CreateDbContext();

            var user = await context.Users.SingleOrDefaultAsync(u => u.UserName == username);
            if (user != null)
            {
                return user.Id;
            }

            user = new User
            {
                UserName = username,
                PasswordHash = _passwordHasher.Hash(password, 50000),
                DisplayName = displayName,
                Permissions = permissions
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return user.Id;
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            await using var context = _contextFactory.CreateDbContext();

            var user = await context.Users.FindAsync(userId);
            if (user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
            }
        }
    }
}
