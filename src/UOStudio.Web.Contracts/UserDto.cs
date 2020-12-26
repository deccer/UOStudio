using System;
using UOStudio.Core;

namespace UOStudio.Web.Contracts
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Permissions Permissions { get; set; }

        public bool IsActive { get; set; }

        public bool IsBlocked { get; set; }
    }
}
