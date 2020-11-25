using System.Data;

namespace UOStudio.Core.Network
{
    public static class PacketIds
    {
        public const int Connect = 1;
        public const int Disconnect = 2;

        public const int ListAccounts = 3;
        public const int CreateAccount = 4;
        public const int DeleteAccount = 5;
        public const int UpdateAccount = 6;

        public const int ListProjects = 7;
        public const int CreateProject = 8;
        public const int DeleteProject = 9;
        public const int UpdateProject = 10;

        public const int JoinProject = 11;
        public const int LeaveProject = 12;
    }
}
