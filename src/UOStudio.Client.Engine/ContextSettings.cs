namespace UOStudio.Client.Engine
{
    public sealed class ContextSettings
    {
        public static readonly ContextSettings Default = new()
        {
            TargetGLVersion = "4.5",
            IsDebugContext = false
        };

        public string TargetGLVersion { get; set; }

        public bool IsDebugContext { get; set; }
    }
}