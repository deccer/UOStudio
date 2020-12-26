using System;

namespace UOStudio.Core
{
    [Flags]
    public enum Permissions
    {
        Viewer = 1,
        Editor = 2,
        Backup = 4,
        Administrator = 8
    }
}
