using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UOStudio.Core;
using UOStudio.Web.Contracts;

namespace UOStudio.Web.Services
{
    public interface IUserService
    {
        Task<UserDto> AuthenticateAsync(string userName, string password);

        Task<IEnumerable<UserDto>> GetAllAsync();

        Task<UserDto> GetUserAsync(Guid userId);

        Task<UserDto> GetUserByNameAndVerifyPasswordAsync(string userName, string passwordHash);

        Task<Guid> CreateUserAsync(string username, string password, string displayName, Permissions permissions);

        Task DeleteUserAsync(Guid userId);
    }
}
