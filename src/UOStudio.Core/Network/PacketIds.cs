namespace UOStudio.Core.Network
{
    public static class PacketIds
    {
        public static class C2S
        {
            public const int Connect = 1;
            public const int Disconnect = 2;
            public const int SystemMessage = 3;
            public const int ChatMessage = 4;

            public const int ListAccounts = 5;
            public const int CreateAccount = 6;
            public const int DeleteAccount = 7;
            public const int UpdateAccount = 8;

            public const int ListProjects = 9;
            public const int CreateProject = 10;
            public const int DeleteProject = 11;
            public const int UpdateProject = 12;

            public const int JoinProject = 13;
            public const int LeaveProject = 14;
        }

        public static class S2C
        {
            public const int ConnectSuccess = 1;
            public const int ConnectFailed = 2;
            public const int RefreshUsers = 3; // when clients connect/disconnect, send new list of clients
            public const int SystemMessage = 4;
            public const int ChatMessage = 5;

            public const int ListAccountsSuccess = 6;
            public const int ListAccountsFailed = 7;
            public const int CreateAccountSuccess = 8;
            public const int CreateAccountFailed = 9;
            public const int DeleteAccountSuccess = 10;
            public const int DeleteAccountFailed = 11;
            public const int UpdateAccountSuccess = 12;
            public const int UpdateAccountFailed = 13;

            public const int ListProjectsSuccess = 14;
            public const int ListProjectsFailed = 15;
            public const int CreateProjectSuccess = 16;
            public const int CreateProjectFailed = 17;
            public const int DeleteProjectSuccess = 18;
            public const int DeleteProjectFailed = 19;
            public const int UpdateProjectSuccess = 20;
            public const int UpdateProjectFailed = 21;

            public const int JoinProjectSuccess = 22;
            public const int JoinProjectFailed = 23;
            public const int LeaveProjectSuccess = 22;
            public const int LeaveProjectFailed = 23;
        }
    }
}
