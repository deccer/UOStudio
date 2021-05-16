namespace UOStudio.Common.Network
{
    public static class PacketIds
    {
        public static class C2S
        {
            public const int JoinProject = 1;
            public const int LeaveProject = 2;
        }

        public static class S2C
        {
            public const int JoinProjectOk = 1;
            public const int JoinProjectFailed = 2;
            public const int LeaveProjectOk = 3;

            public static class Broadcast
            {
            }
        }
    }
}
