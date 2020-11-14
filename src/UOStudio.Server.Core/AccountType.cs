using System;

namespace UOStudio.Server.Core
{
    [Flags]
    public enum AccountType
    {
        User,
        Moderator,
        Administrator,
        Backup
    }
}
