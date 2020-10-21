namespace UOStudio.Client.Core.Settings
{
    public sealed class AppSettings
    {
        public VideoSettings Video { get; }

        public AppSettings()
        {
            Video = new VideoSettings();
        }
    }
}
