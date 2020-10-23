namespace UOStudio.Client.Core.Settings
{
    public sealed class AppSettings
    {
        public GeneralSettings General { get; }

        public VideoSettings Video { get; }

        public AppSettings()
        {
            General = new GeneralSettings();
            Video = new VideoSettings();
        }
    }
}
