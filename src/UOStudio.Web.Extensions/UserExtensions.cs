using System.Collections.Generic;
using System.Linq;
using UOStudio.Web.Contracts;
using UOStudio.Web.Data.Entities;

namespace UOStudio.Web.Extensions
{
    public static class UserExtensions
    {
        public static IEnumerable<User> WithoutPasswords(this IEnumerable<User> users)
        {
            return users.Select(x => x.WithoutPassword());
        }

        public static User WithoutPassword(this User user)
        {
            user.PasswordHash = null;
            return user;
        }

        public static IEnumerable<UserDto> ToDto(this IEnumerable<User> users) => users.Select(u => u.ToDto());

        public static IEnumerable<UserDto> ToDto(this IQueryable<User> users) => users.Select(u => u.ToDto());

        public static UserDto ToDto(this User user) => new UserDto
        {
            Id = user.Id,
            Name = user.UserName,
            Permissions = user.Permissions
        };
    }
}
