using System;
using UOStudio.Core;

namespace UOStudio.Web.Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public string DisplayName { get; set; }

        public Permissions Permissions { get; set; }

        public UserStatus Status { get; set; }

        public bool IsBlocked() => Status == UserStatus.Blocked;
    }
}
