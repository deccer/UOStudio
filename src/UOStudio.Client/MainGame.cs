using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;
using UOStudio.Client.Screens;
using UOStudio.Client.Services;
using UOStudio.Client.UI;
using Num = System.Numerics;

namespace UOStudio.Client
{
    internal sealed class MainGame : Game
    {
        private readonly ILogger _logger;
        private readonly IScreenHandler _screenHandler;
        private readonly INetworkClient _networkClient;
        private readonly IWindowProvider _windowProvider;
        private readonly IContext _context;
        private readonly FrameTimeCalculator _ftc;

        private readonly Color _clearColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);

        private ImGuiRenderer _imGuiRenderer;

        private KeyboardState _currentKeyboardState;
        private MouseState _currentMouseState;
        private bool _isWindowFocused;

        public MainGame(
            ILogger logger,
            GraphicsSettings graphicsSettings,
            IScreenHandler screenHandler,
            INetworkClient networkClient,
            IWindowProvider windowProvider,
            IContext context)
        {
            _logger = logger;
            FNALoggerEXT.LogError = message => _logger.Error("FNA: {@Message}", message);
            FNALoggerEXT.LogInfo = message => _logger.Information("FNA: {@Message}", message);
            FNALoggerEXT.LogWarn = message => _logger.Warning("FNA: {@Message}", message);

            _screenHandler = screenHandler;
            _networkClient = networkClient;
            _windowProvider = windowProvider;
            _context = context;
            _windowProvider.Load();
            _ftc = new FrameTimeCalculator(this);

            Content.RootDirectory = "Content";
            Window.Title = "FNA-Bootstrap";
            Window.AllowUserResizing = true;
            Activated += (_, _) => { _isWindowFocused = true; };
            Deactivated += (_, _) =>
            {
                _isWindowFocused = false;
                Window.Title = $"UO-Studio {graphicsSettings.Backend}";
            };

            var graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
                PreferMultiSampling = true,
                GraphicsProfile = GraphicsProfile.HiDef,
                SynchronizeWithVerticalRetrace = false
            };

            IsMouseVisible = true;
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            var sw = Stopwatch.StartNew();
            _logger.Information("Initializing...");

            _imGuiRenderer = new ImGuiRenderer(this);
            _imGuiRenderer.RebuildFontAtlas();
            _screenHandler.LoadScreen(new LoginScreen(this, _logger, _windowProvider, _networkClient, _context));
            _screenHandler.Initialize(this);

            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();

            base.Initialize();
            sw.Stop();
            _logger.Information("Initializing...Done. Took {@TotalSeconds}s", sw.Elapsed.TotalSeconds);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!_isWindowFocused)
            {
                return;
            }

            _ftc.Calculate(gameTime);

            GraphicsDevice.Clear(_clearColor);
            _screenHandler.Draw(gameTime, _imGuiRenderer);

            base.Draw(gameTime);
        }

        protected override void LoadContent()
        {
            _logger.Information("Content - Loading...");
            base.LoadContent();
            _screenHandler.LoadContent(Content);
            _logger.Information("Content - Loading...Done");
        }

        protected override void UnloadContent()
        {
            _logger.Information("Content - Unloading...");
            _screenHandler.UnloadContent();
            base.UnloadContent();
            _logger.Information("Content - Unloading...Done");
        }

        protected override void Update(GameTime gameTime)
        {
            _networkClient.Update();

            if (!_isWindowFocused)
            {
                return;
            }
            _imGuiRenderer.UpdateInput();
            _screenHandler.Update(gameTime);

            Window.Title = $"MSPF: {_ftc.AverageFrameTime / 1000.0f:F2} | FPS: {_ftc.Fps:F0}";

            var previousKeyboardState = _currentKeyboardState;
            var previousMouseState = _currentMouseState;
            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();

            if (_currentKeyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);
        }
    }
}
