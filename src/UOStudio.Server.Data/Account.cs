using System;
using System.ComponentModel.DataAnnotations;

namespace UOStudio.Server.Data
{
    public class Account
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(128)]
        [Required]
        public string UserName { get; set; }

        [MaxLength(128)]
        public string DisplayName { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public AccountType Type { get; set; }

        public AccountStatus Status { get; set; }

        public byte[] Version { get; set; }

        public bool IsActive() => Status == AccountStatus.Active;

        public bool IsBlocked() => Status == AccountStatus.Blocked;

        public bool IsAdministrator() => Type == AccountType.Administrator;

        public bool IsUser() => Type.HasFlag(AccountType.User);
    }
}
