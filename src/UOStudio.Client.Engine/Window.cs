using System.Runtime.InteropServices;
using Serilog;
using UOStudio.Client.Engine.Input;
using UOStudio.Client.Engine.Native;
using UOStudio.Client.Engine.Platform;

namespace UOStudio.Client.Engine
{
    internal sealed class Window : IWindow
    {
        private readonly ILogger _logger;
        private readonly string _title;
        private readonly WindowSettings _windowSettings;
        private readonly Sdl.WindowFlags _windowFlags;
        private IntPtr _windowHandle;

        public int Width { get; private set; }

        public int Height { get; private set; }

        public IntPtr Handle => _windowHandle;

        public string Title
        {
            get => Sdl.GetWindowTitle(_windowHandle);
            set => Sdl.SetWindowTitle(_windowHandle, value);
        }

        public Window(
            ILogger logger,
            string title,
            WindowSettings windowSettings)
        {
            _logger = logger;
            _title = title;
            _windowSettings = windowSettings;
            _windowFlags = Sdl.WindowFlags.SupportOpenGL | Sdl.WindowFlags.AllowHighDpi;

            switch (_windowSettings.WindowMode)
            {
                case WindowMode.Windowed or WindowMode.MaximizedWindow:
                    _windowFlags |= Sdl.WindowFlags.Resizable;
                    break;
                case WindowMode.ExclusiveFullscreen or WindowMode.FullscreenWindow:
                    _windowFlags |= Sdl.WindowFlags.Borderless;
                    break;
            }

            if (_windowSettings.WindowMode is WindowMode.MaximizedWindow)
            {
                _windowFlags |= Sdl.WindowFlags.Maximized;
            }

            if (!_windowSettings.Visible)
            {
                _windowFlags |= Sdl.WindowFlags.Hidden;
            }
        }

        public bool Initialize()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Windows.SetProcessDpiAwareness(Windows.ProcessDpiAwareness.SystemDpiAware);
            }

            var displayBounds = Sdl.GetDisplayBounds(0);
            Width = _windowSettings.WindowMode is WindowMode.ExclusiveFullscreen or WindowMode.FullscreenWindow
                ? displayBounds.Width
                : _windowSettings.ResolutionWidth;
            Height = _windowSettings.WindowMode is WindowMode.ExclusiveFullscreen or WindowMode.FullscreenWindow
                ? displayBounds.Height
                : _windowSettings.ResolutionHeight;

            _windowHandle = Sdl.CreateWindow(
                _title,
                Width,
                Height,
                _windowFlags);
            if (_windowHandle == IntPtr.Zero)
            {
                _logger.Error("SDL: CreateWindow failed");
                Dispose();
                return false;
            }

            if (_windowSettings.WindowMode == WindowMode.MaximizedWindow)
            {
                Sdl.GetWindowSize(_windowHandle, out var windowWidth, out var windowHeight);
                Width = windowWidth;
                Height = windowHeight;
            }
            _windowSettings.ResolutionWidth = Width;
            _windowSettings.ResolutionHeight = Height;

            Mouse.WindowHandle = _windowHandle;
            return true;
        }

        public void Dispose()
        {
            if (_windowHandle != IntPtr.Zero)
            {
                Sdl.MakeCurrent(_windowHandle, IntPtr.Zero);
                Sdl.DestroyWindow(_windowHandle);
            }
        }
    }
}
