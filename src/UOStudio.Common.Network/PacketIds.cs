namespace UOStudio.Common.Network
{
    public static class PacketIds
    {
        public static class C2S
        {
            public const int JoinWorld = 1;
            public const int LeaveWorld = 2;

            public const int Chat = 3;
            public const int RequestChunk = 4;
        }

        public static class S2C
        {
            public const int JoinWorldOk = 1;
            public const int JoinWorldFailed = 2;
            public const int LeaveWorldOk = 3;
            public const int LeaveWorldFailed = 4;

            public const int RequestedChunk = 5;

            public const int Chat = 6;

            public static class Broadcast
            {
            }
        }
    }
}
