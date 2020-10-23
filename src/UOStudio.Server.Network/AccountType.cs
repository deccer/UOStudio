using System;

namespace UOStudio.Server.Network
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
