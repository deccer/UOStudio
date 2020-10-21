namespace UOStudio.Server.Core.Settings
{
    public class NetworkSettings
    {
        public int Port { get; set; }

        public NetworkSettings()
        {
            Port = 9050;
        }
    }
}
