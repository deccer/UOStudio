namespace UOStudio.Client.Engine
{
    public sealed class WindowSettings
    {
        public static readonly WindowSettings Default = new()
        {
            ResolutionWidth = 1280,
            ResolutionHeight = 720,
            ResolutionScale = 1.0f,
            WindowMode = WindowMode.Windowed,
            IsVsyncEnabled = true,
            UpdatesPerSecond = 60
        };

        public int ResolutionWidth { get; set; }

        public int ResolutionHeight { get; set; }

        public float ResolutionScale { get; set; }

        public WindowMode WindowMode { get; set; }

        public bool IsVsyncEnabled { get; set; }

        public int UpdatesPerSecond { get; set; }
    }
}
