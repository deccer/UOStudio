namespace UOStudio.Client.Engine
{
    public interface IWindowFactory
    {
        IWindow CreateWindow(string title, WindowSettings windowSettings);

        IWindow CreateWindow(string title);
    }
}
