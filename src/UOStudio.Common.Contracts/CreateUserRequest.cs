using System.Collections.Generic;

namespace UOStudio.Common.Contracts
{
    public class CreateUserRequest
    {
        public string Name { get; set; }

        public string Password { get; set; }

        public IList<string> Permissions { get; set; }
    }
}
