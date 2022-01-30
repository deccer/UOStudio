using System.Diagnostics;
using System.Runtime.InteropServices;
using Serilog;
using UOStudio.Client.Engine.Graphics;
using UOStudio.Client.Engine.Input;
using UOStudio.Client.Engine.Native;
using UOStudio.Client.Engine.Native.OpenGL;
using UOStudio.Client.Engine.Platform;
using Buffer = System.Buffer;

namespace UOStudio.Client.Engine
{
    public class Application : IApplication
    {
        private readonly bool _listExtensions = false;

        private readonly ILogger _logger;
        private readonly WindowSettings _windowSettings;
        private readonly ContextSettings _contextSettings;

        private readonly IGraphicsDevice _graphicsDevice;

        private readonly bool _showUpdatesPerSecond = true;
        private readonly int _showUpdatesPerSecondRate = 1000;

        private IntPtr _windowHandle;
        private int _updatesPerSecond;
        private int _framesPerSecond;
        private bool _isRunning;

        protected int FrameWidth;
        protected int FrameHeight;
        protected int ResolutionWidth;
        protected int ResolutionHeight;

        public bool IsFocused { get; private set; }

        public string Title
        {
            get => Sdl.GetWindowTitle(_windowHandle);
            set => Sdl.SetWindowTitle(_windowHandle, value);
        }

        protected Application(
            ILogger logger,
            WindowSettings windowSettings,
            ContextSettings contextSettings,
            IGraphicsDevice graphicsDevice)
        {
            _logger = logger.ForContext<Application>();
            _windowSettings = windowSettings;
            _contextSettings = contextSettings;
            _graphicsDevice = graphicsDevice;

            FrameWidth = (int)(_windowSettings.ResolutionWidth * _windowSettings.ResolutionScale);
            FrameHeight = (int)(_windowSettings.ResolutionHeight * _windowSettings.ResolutionScale);
            ResolutionWidth = _windowSettings.ResolutionWidth;
            ResolutionHeight = _windowSettings.ResolutionHeight;
        }

        public void Run()
        {
            PrintSystemInformation();
            if (!Initialize())
            {
                _logger.Error("App: Initializing failed");
                return;
            }
            _logger.Debug("App: Initialized");

            PrintOpenGLInformation();

            if (!Load())
            {
                _logger.Error("App: Loading failed");
                return;
            }
            _logger.Debug("App: Loaded");
            _isRunning = true;

            var stopwatch = Stopwatch.StartNew();
            var accumulator = 0f;
            var currentTime = stopwatch.ElapsedMilliseconds;
            var lastTime = currentTime;
            var nextUpdate = lastTime + _showUpdatesPerSecondRate;
            var updateRate = 1.0f / _windowSettings.UpdatesPerSecond;

            while (_isRunning)
            {
                currentTime = stopwatch.ElapsedMilliseconds;
                var deltaTime = currentTime - lastTime;
                var lastRenderTimeInSeconds = deltaTime / (float)_showUpdatesPerSecondRate;
                accumulator += lastRenderTimeInSeconds;
                lastTime = currentTime;

                while (accumulator >= updateRate)
                {
                    var currentKeyboardState = Sdl.GetKeyboardState(out var numberOfKeys);
                    Marshal.Copy(currentKeyboardState, Keyboard._keyState, 0, numberOfKeys);

                    HandleEvents();
                    if (!_isRunning)
                    {
                        break;
                    }

                    Update(currentTime, updateRate);

                    Buffer.BlockCopy(Keyboard._keyState, 0, Keyboard._previousKeyState, 0, Keyboard._keyState.Length);
                    accumulator -= updateRate;
                    _updatesPerSecond++;
                }

                Timer.BeginFrame();
                Render(currentTime, updateRate);
                Timer.EndFrame();
                _framesPerSecond++;
                Sdl.SwapWindow(_windowHandle);

                if (_showUpdatesPerSecond && stopwatch.ElapsedMilliseconds > nextUpdate)
                {
                    _logger.Debug("FPS: {@Fps} UPS: {@Ups} UR: {@UR}", _framesPerSecond, _updatesPerSecond, updateRate);
                    _updatesPerSecond = 0;
                    _framesPerSecond = 0;
                    nextUpdate = stopwatch.ElapsedMilliseconds + _showUpdatesPerSecondRate;
                }
            }

            Unload();
            _graphicsDevice.Dispose();
            UnInitialize();
        }

        protected virtual void FocusGained()
        {
        }

        protected virtual void FocusLost()
        {
        }

        protected virtual bool Initialize()
        {
            if (!Sdl.Init(Sdl.InitFlags.Video))
            {
                _logger.Error("SDL: Init failed");
                return false;
            }

            var windowFlags = Sdl.WindowFlags.SupportOpenGL | Sdl.WindowFlags.AllowHighDpi | Sdl.WindowFlags.InputGrabbed;
            switch (_windowSettings.WindowMode)
            {
                case WindowMode.Windowed or WindowMode.MaximizedWindow:
                    windowFlags |= Sdl.WindowFlags.Resizable;
                    break;
                case WindowMode.ExclusiveFullscreen or WindowMode.FullscreenWindow:
                    windowFlags |= Sdl.WindowFlags.Borderless;
                    break;
            }

            if (_windowSettings.WindowMode is WindowMode.MaximizedWindow)
            {
                windowFlags |= Sdl.WindowFlags.Maximized;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Windows.SetProcessDpiAwareness(Windows.ProcessDpiAwareness.SystemDpiAware);
            }

            var displayBounds = Sdl.GetDisplayBounds(0);
            var windowWidth = _windowSettings.WindowMode is WindowMode.ExclusiveFullscreen or WindowMode.FullscreenWindow
                ? displayBounds.Width
                : _windowSettings.ResolutionWidth;
            var windowHeight = _windowSettings.WindowMode is WindowMode.ExclusiveFullscreen or WindowMode.FullscreenWindow
                ? displayBounds.Height
                : _windowSettings.ResolutionHeight;

            _windowHandle = Sdl.CreateWindow(
                "EngineKit",
                windowWidth,
                windowHeight,
                windowFlags);
            if (_windowHandle == IntPtr.Zero)
            {
                _logger.Error("SDL: CreateWindow failed");
                UnInitialize();
                return false;
            }

            if (_windowSettings.WindowMode == WindowMode.MaximizedWindow)
            {
                Sdl.GetWindowSize(_windowHandle, out windowWidth, out windowHeight);
            }
            _windowSettings.ResolutionWidth = windowWidth;
            _windowSettings.ResolutionHeight = windowHeight;

            Mouse.WindowHandle = _windowHandle;

            if (!_graphicsDevice.Initialize(_contextSettings, _windowHandle))
            {
                _logger.Error("App: Unable to initialize graphics device");
                return false;
            }

            _graphicsDevice.VSync = _windowSettings.IsVsyncEnabled;
            Resized(windowWidth, windowHeight);
            return true;
        }

        protected virtual bool Load()
        {
            return true;
        }

        protected virtual void Render(float elapsedTime, float deltaTime)
        {
        }

        protected virtual void Resized(int width, int height)
        {
            FrameWidth = (int)(width * _windowSettings.ResolutionScale);
            FrameHeight = (int)(height * _windowSettings.ResolutionScale);
            ResolutionWidth = _windowSettings.ResolutionWidth;
            ResolutionHeight = _windowSettings.ResolutionHeight;
        }

        protected virtual void UnInitialize()
        {
            if (_windowHandle != IntPtr.Zero)
            {
                Sdl.MakeCurrent(_windowHandle, IntPtr.Zero);
                Sdl.DestroyWindow(_windowHandle);
            }
            Sdl.Quit();
        }

        protected virtual void Unload()
        {
        }

        protected virtual void Update(float elapsedTime, float deltaTime)
        {
        }

        protected void Close()
        {
            _isRunning = false;
        }

        private void HandleEvents()
        {
            if (Sdl.PollEvent(out var ev) <= 0)
            {
                return;
            }

            switch (ev.Type)
            {
                case Sdl.EventType.Quit:
                    _isRunning = false;
                    return;
                case Sdl.EventType.Mousewheel:
                    // ReSharper disable once S2696
                    Mouse.InternalMouseWheel += ev.Wheel.Y * 120;
                    break;
                case Sdl.EventType.WindowEvent:
                    HandleWindowEvents(ev.Window);
                    break;
            }
        }

        private void HandleWindowEvents(Sdl.SdlWindowEvent windowEvent)
        {
            switch (windowEvent.WindowEvent)
            {
                case Sdl.WindowEventId.Resized:
                    _logger.Debug("SDL: Resized");
                    Resized(windowEvent.data1, windowEvent.data2);
                    break;
                case Sdl.WindowEventId.Shown:
                    _logger.Debug("SDL: Shown");
                    break;
                case Sdl.WindowEventId.Hidden:
                    _logger.Debug("SDL: Hidden");
                    break;
                case Sdl.WindowEventId.SizeChanged:
                    _logger.Debug("SDL: SizeChanged");
                    break;
                case Sdl.WindowEventId.Minimized:
                    _logger.Debug("SDL: Minimized");
                    break;
                case Sdl.WindowEventId.Maximized:
                    _logger.Debug("SDL: Maximized");
                    break;
                case Sdl.WindowEventId.Restored:
                    _logger.Debug("SDL: Restored");
                    break;
                case Sdl.WindowEventId.FocusGained:
                    _logger.Debug("SDL: FocusGained");
                    IsFocused = true;
                    FocusGained();
                    break;
                case Sdl.WindowEventId.FocusLost:
                    _logger.Debug("SDL: FocusLost");
                    IsFocused = false;
                    FocusLost();
                    break;
            }
        }

        private void PrintSystemInformation()
        {
            _logger.Debug("OS: Description - {@OSDescription}", RuntimeInformation.OSDescription);
            _logger.Debug("OS: Architecture - {@OSArchitecture}", RuntimeInformation.OSArchitecture);

            _logger.Debug("RT: Framework - {@FrameworkDescription}", RuntimeInformation.FrameworkDescription);
            _logger.Debug("RT: Runtime Identifier - {@RuntimeIdentifier}", RuntimeInformation.RuntimeIdentifier);
            _logger.Debug("RT: Process Architecture - {@ProcessArchitecture}", RuntimeInformation.ProcessArchitecture);
        }

        private void PrintOpenGLInformation()
        {
            var numExtensions = GL.GetInteger(GL.ValueName.NumExtensions);
            _logger.Debug("GL: Vendor - {@Vendor}", GL.GetString(GL.StringName.Vendor));
            _logger.Debug("GL: Renderer - {@Renderer}", GL.GetString(GL.StringName.Renderer));
            _logger.Debug("GL: Version - {@GLVersion}", GL.GetString(GL.StringName.Version));
            _logger.Debug("GL: GLSL Version - {@GlslVersion}", GL.GetString(GL.StringName.ShadingLanguageVersion));
            _logger.Debug("GL: Extensions: {@NumExtensions}", numExtensions);
            if (!_listExtensions)
            {
                return;
            }

            var extensions = new List<string>(numExtensions);
            for (var i = 0; i < numExtensions; i++)
            {
                var extension = GL.GetString(GL.StringName.Extensions, i);
                //_logger.Debug("GL: Extension - {@Extension}", extension);
                extensions.Add(extension);
            }

            extensions.Sort();
            foreach (var extension in extensions)
            {
                _logger.Debug("GL: Extension - {@Extension}", extension);
            }
        }
    }
}
