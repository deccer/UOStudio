using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using UOStudio.Common.Contracts;

namespace UOStudio.Server.Data.Extensions
{
    public static class UserExtensions
    {
        public static UserDto ToDto(this User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name)
            };
            claims.AddRange(user.Permissions.AsEnumerable().Select(permission => new Claim(ClaimTypes.Role, permission.Name)));

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Claims = new List<Claim>(claims)
            };
        }
    }
}
