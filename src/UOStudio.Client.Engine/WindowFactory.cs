using Serilog;

namespace UOStudio.Client.Engine
{
    internal sealed class WindowFactory : IWindowFactory
    {
        private readonly ILogger _logger;
        private readonly WindowSettings _windowSettings;

        public WindowFactory(
            ILogger logger,
            WindowSettings windowSettings)
        {
            _logger = logger.ForContext<WindowFactory>();
            _windowSettings = windowSettings;
        }

        public IWindow CreateWindow(string title, WindowSettings windowSettings)
        {
            return new Window(_logger, title, windowSettings);
        }

        public IWindow CreateWindow(string title)
        {
            return new Window(_logger, title, _windowSettings);
        }
    }
}
