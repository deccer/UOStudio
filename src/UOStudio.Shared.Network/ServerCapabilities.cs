using System;

namespace UOStudio.Shared.Network
{
    [Flags]
    public enum ServerCapabilities
    {
        AccountManagement,
        SingleUserOnly
    }
}
