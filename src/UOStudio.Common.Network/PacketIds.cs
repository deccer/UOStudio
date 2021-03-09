namespace UOStudio.Common.Network
{
    public static class PacketIds
    {
        public static class C2S
        {
            public const int Login = 1;
            public const int Logout = 2;

            public const int CreateProject = 3;
            public const int GetProjectDetailsByProjectId = 4;
            public const int GetProjectDetailsByProjectName = 5;
        }

        public static class S2C
        {
            public const int LoginOk = 0;
            public const int LoginError = 1;

            public const int CreateProjectOk = 2;
            public const int CreateProjectError = 3;

            public const int GetProjectDetailsOk = 4;
            public const int GetProjectDetailsError = 5;

            public static class Broadcast
            {
                public const int Logout = 2; // tell clients that a client has signed out
            }
        }
    }
}
