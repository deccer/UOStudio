using System.Collections.Generic;
using System.Linq;
using UOStudio.Core;

namespace UOStudio.Server.Network
{
    public sealed class AccountStore : IAccountStore
    {
        private readonly ILoader _loader;
        private readonly ISaver _saver;
        private List<Account> _accounts;

        private const string AccountsFileName = "accounts.json";

        public AccountStore(ILoader loader, ISaver saver)
        {
            _loader = loader;
            _saver = saver;

            _accounts = new List<Account>();
        }

        public void Load()
        {
            _accounts = _loader.Load<List<Account>>(AccountsFileName);
        }

        public void Save()
        {
            _saver.Save(AccountsFileName, _accounts);
        }

        public Account GetAccount(string userName) => _accounts.FirstOrDefault(account => account.UserName == userName);

        public IReadOnlyCollection<Account> GetAccounts() => _accounts;

        public void AddAccount(string userName, AccountType accountType)
        {
            if (GetAccount(userName) != null)
            {
                return;
            }

            var account = new Account
            {
                UserName = userName,
                DisplayName = userName,
                Type = accountType,
                Status = AccountStatus.Active
            };

            _accounts.Add(account);
        }

        public void BlockAccount(string userName)
        {
            var account = GetAccount(userName);
            if (account == null)
            {
                return;
            }

            account.Status = AccountStatus.Blocked;
        }

        public void RemoveAccount(string userName)
        {
            var account = GetAccount(userName);
            if (account == null)
            {
                return;
            }

            _accounts.Remove(account);
        }
    }
}
