namespace UOStudio.Server.Core.Settings
{
    public sealed class AppSettings
    {
        public NetworkSettings Network { get; private set; }

        public AppSettings()
        {
            Network = new NetworkSettings();
        }
    }
}
