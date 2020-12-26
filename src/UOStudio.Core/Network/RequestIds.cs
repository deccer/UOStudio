namespace UOStudio.Core.Network
{
    public static class RequestIds
    {
        public enum C2S : int
        {
            Connect = 1,
            Disconnect = 2,
            SystemMessage = 3,
            ChatMessage = 4,
            JoinProject = 13,
            LeaveProject = 14
        }

        public enum S2C : int
        {
            ConnectSuccess = 1,
            ConnectFailed = 2,
            RefreshUsers = 3, // when clients connect/disconnect, send new list of clients
            SystemMessage = 4,
            ChatMessage = 5,
            JoinProjectSuccess = 22,
            JoinProjectFailed = 23,
            LeaveProjectSuccess = 24,
            LeaveProjectFailed = 25,
        }
    }
}
