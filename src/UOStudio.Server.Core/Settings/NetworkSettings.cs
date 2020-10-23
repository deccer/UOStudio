namespace UOStudio.Server.Core.Settings
{
    public class NetworkSettings
    {
        public int Port { get; set; }

        public int ConnectedPeersCount { get; set; }

        public NetworkSettings()
        {
            ConnectedPeersCount = 10;
            Port = 9050;
        }
    }
}
