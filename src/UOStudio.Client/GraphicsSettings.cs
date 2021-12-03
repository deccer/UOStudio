namespace UOStudio.Client
{
    public class GraphicsSettings
    {
        public const string Key = "Graphics";

        public GraphicsBackend Backend { get; set; }

        public int ResolutionWidth { get; set; }

        public int ResolutionHeight { get; set; }

        public bool IsVsyncEnabled { get; set; }

        public bool IsMultiSamplingEnabled { get; set; }
    }
}
