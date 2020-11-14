namespace UOStudio.Server.Core
{
    public class Account
    {
        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public AccountType Type { get; set; }

        public AccountStatus Status { get; set; }

        public bool IsActive() => Status == AccountStatus.Active;

        public bool IsBlocked() => Status == AccountStatus.Blocked;

        public bool IsAdministrator() => Type == AccountType.Administrator;

        public bool IsUser() => Type.HasFlag(AccountType.User);
    }
}
