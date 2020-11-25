using System;

namespace UOStudio.Server.Data
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
