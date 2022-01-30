namespace UOStudio.Client.Engine
{
    public interface IWindow : IDisposable
    {
        int Width { get; }

        int Height { get; }

        IntPtr Handle { get; }

        string Title { get; set; }

        bool Initialize();
    }
}
