using System.Collections.Generic;
using System.Security.Claims;

namespace UOStudio.Common.Contracts
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Claim> Claims { get; set; }
    }
}
