namespace UOStudio.Server.Core.Settings
{
    public sealed class AppSettings
    {
        public GeneralSettings General { get; }

        public NetworkSettings Network { get; }

        public AppSettings()
        {
            Network = new NetworkSettings();
            General = new GeneralSettings();
        }
    }
}
