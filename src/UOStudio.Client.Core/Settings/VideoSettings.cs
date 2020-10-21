namespace UOStudio.Client.Core.Settings
{
    public sealed class VideoSettings
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public bool IsFullScreen { get; set; }

        public VideoSettings()
        {
            IsFullScreen = false;
            Width = 1920;
            Height = 1080;
        }
    }
}
