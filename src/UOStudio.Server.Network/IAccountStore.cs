using System.Collections.Generic;

namespace UOStudio.Server.Network
{
    public interface IAccountStore
    {
        public Account GetAccount(string userName);

        public IReadOnlyCollection<Account> GetAccounts();

        public void AddAccount(string userName, AccountType accountType);

        public void Load();

        public void Save();
    }
}
