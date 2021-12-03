using System;
using System.Diagnostics;
using System.Drawing;
using ImGuiNET;
using LiteNetLib;
using Microsoft.Extensions.Options;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;
using UOStudio.Client.Core.Extensions;
using UOStudio.Client.Services;
using UOStudio.Client.UI;
using UOStudio.Client.Worlds;
using UOStudio.Common.Contracts;
using UOStudio.Common.Network;
using Color = Microsoft.Xna.Framework.Color;
using Num = System.Numerics;
using Point = Microsoft.Xna.Framework.Point;

namespace UOStudio.Client
{
    internal sealed class MainGame : Game
    {
        private readonly ILogger _logger;

        private readonly ClientStartParameters _clientStartParameters;

        private readonly INetworkClient _networkClient;
        private readonly IWindowProvider _windowProvider;
        private readonly IWorldProvider _worldProvider;
        private readonly IContext _context;
        private readonly FrameTimeCalculator _ftc;
        private readonly IOptions<GraphicsSettings> _graphicsSettings;
        private readonly IWorldRenderer _worldRenderer;

        private readonly Camera _camera;

        private World _currentWorld;

        private readonly Color _clearColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);

        private ImGuiRenderer _imGuiRenderer;

        private SpriteBatch _spriteBatch;

        private KeyboardState _previousKeyboardState;
        private MouseState _previousMouseState;
        private bool _isWindowFocused;

        public MainGame(
            ILogger logger,
            ClientStartParameters clientStartParameters,
            IOptions<GraphicsSettings> graphicsSettings,
            INetworkClient networkClient,
            IWindowProvider windowProvider,
            IWorldProvider worldProvider,
            IWorldRenderer worldRenderer,
            IContext context)
        {
            _logger = logger;
            _clientStartParameters = clientStartParameters;
            _graphicsSettings = graphicsSettings;
            FNALoggerEXT.LogError = message => _logger.Error("FNA: {@Message}", message);
            FNALoggerEXT.LogInfo = message => _logger.Information("FNA: {@Message}", message);
            FNALoggerEXT.LogWarn = message => _logger.Warning("FNA: {@Message}", message);

            _networkClient = networkClient;
            _networkClient.Connected += NetworkClientConnected;
            _networkClient.Disconnected += NetworkClientDisconnected;
            _networkClient.ChunkReceived += NetworkClientChunkReceived;

            _windowProvider = windowProvider;
            _context = context;
            _windowProvider.Load();
            _ftc = new FrameTimeCalculator(this);
            _worldProvider = worldProvider;
            _worldRenderer = worldRenderer;

            Content.RootDirectory = nameof(Content);
            Window.Title = "UO-Studio";
            Window.AllowUserResizing = true;
            Activated += WindowActivated;
            Deactivated += WindowDeactivated;

            _ = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = graphicsSettings.Value.ResolutionWidth,
                PreferredBackBufferHeight = graphicsSettings.Value.ResolutionHeight,
                PreferMultiSampling = graphicsSettings.Value.IsMultiSamplingEnabled,
                GraphicsProfile = GraphicsProfile.HiDef,
                SynchronizeWithVerticalRetrace = graphicsSettings.Value.IsVsyncEnabled
            };

            _camera = new Camera(graphicsSettings.Value.ResolutionWidth, graphicsSettings.Value.ResolutionHeight)
            {
                Mode = CameraMode.Perspective
            };

            IsMouseVisible = true;
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            var sw = Stopwatch.StartNew();
            _logger.Information("App: Initializing...");

            _imGuiRenderer = new ImGuiRenderer(this);
            _imGuiRenderer.RebuildFontAtlas();

            _previousKeyboardState = Keyboard.GetState();
            _previousMouseState = Mouse.GetState();

            base.Initialize();

            _currentWorld = _worldProvider.GetWorld(_clientStartParameters.ProjectId);

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            sw.Stop();
            _logger.Information("App: Initializing...Done. Took {@TotalSeconds}s", sw.Elapsed.TotalSeconds);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!_isWindowFocused)
            {
                return;
            }

            GraphicsDevice.Clear(_clearColor);
            var r = new RectangleF(32, 32, 196, 32);
            var c = r.Contains(new PointF(_previousMouseState.X, _previousMouseState.Y))
                ? _previousMouseState.LeftButton == ButtonState.Pressed
                    ? Color.OrangeRed
                    : Color.Yellow
                : Color.Peru;
            var t = r.Contains(new PointF(_previousMouseState.X, _previousMouseState.Y))
                ? 4.0f
                : 1.0f;

            _spriteBatch.Begin();
            _spriteBatch.DrawRectangle(r, c, t);
            _spriteBatch.End();

            _worldRenderer.Draw(GraphicsDevice, _currentWorld, _camera);

            _imGuiRenderer.BeginLayout(gameTime);
            DrawUserInterface(gameTime);

            ImGui.ShowDemoWindow();

            _windowProvider.Draw();
            _imGuiRenderer.EndLayout();

            base.Draw(gameTime);
        }

        protected void DrawUserInterface(GameTime gameTime)
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("File"))
                    {
                        if (ImGui.MenuItem("Quit"))
                        {
                            Exit();
                        }

                        ImGui.EndMenu();
                    }

                    ImGui.EndMenuBar();
                }

                ImGui.EndMainMenuBar();
            }
        }

        protected override void LoadContent()
        {
            _logger.Information("Content: Loading...");
            base.LoadContent();
            _worldRenderer.LoadContent(Content, GraphicsDevice);
            _logger.Information("Content: Loading...Done");

            _networkClient.Connect("localhost", 9050);
        }

        protected override void UnloadContent()
        {
            _networkClient.Disconnect();

            _logger.Information("Content: Unloading...");
            base.UnloadContent();
            _logger.Information("Content: Unloading...Done");
        }

        protected override void Update(GameTime gameTime)
        {
            _networkClient.Update();

            if (!_isWindowFocused)
            {
                return;
            }
            _ftc.Calculate(gameTime);
            _imGuiRenderer.UpdateInput();

            Window.Title = $"MSPF: {_ftc.AverageFrameTime / 10000.0f:F2} | FPS: {_ftc.Fps:F0}";

            var currentKeyboardState = Keyboard.GetState();
            var currentMouseState = Mouse.GetState();
            var cameraWasMoved = _camera.Update(
                Window.ClientBounds.Width,
                Window.ClientBounds.Height,
                currentKeyboardState,
                currentMouseState,
                _previousKeyboardState,
                _previousMouseState);

            if (cameraWasMoved)
            {
                var cameraChunkPositionX = MathF.Floor(_camera.Position.X / ChunkData.ChunkSize);
                var cameraChunkPositionY = MathF.Floor(_camera.Position.Y / ChunkData.ChunkSize);
                var cameraChunkPositionZ = MathF.Floor(_camera.Position.Z / ChunkData.ChunkSize);
                _logger.Debug("Camera: {@CamChkX}:{@CamChkY}:{@CamChkZ}", cameraChunkPositionX, cameraChunkPositionY, cameraChunkPositionZ);
                var chunkPosition = new Point((int)cameraChunkPositionX, (int)cameraChunkPositionY);
                _networkClient.RequestChunk(_clientStartParameters.ProjectId, chunkPosition);
            }

            if (currentKeyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            _previousKeyboardState = currentKeyboardState;
            _previousMouseState = currentMouseState;

            base.Update(gameTime);
        }

        private static string[] GetThemes()
        {
            return new[]
            {
                "Light",
                "Dark",
                "Custom"
            };
        }

        private static string[] GetLanguages()
        {
            return new[]
            {
                "en-US"
            };
        }

        private void NetworkClientDisconnected()
        {
            _logger.Debug("Network: Disconnected");
        }

        private void NetworkClientConnected(NetPeer peer)
        {
            _logger.Debug("Network: Connected");
        }

        private void NetworkClientChunkReceived(ChunkData chunkData)
        {
            var chunk = _worldProvider.GetChunk(_clientStartParameters.ProjectId, chunkData.Position);
            chunk.UpdateWorldChunk(ref chunkData);
            _worldRenderer.UpdateChunk(chunk);
        }

        private void WindowActivated(object sender, EventArgs eventArgs)
        {
            _isWindowFocused = true;
        }

        private void WindowDeactivated(object sender, EventArgs eventArgs)
        {
            _isWindowFocused = false;
            Window.Title = $"UO-Studio {_graphicsSettings.Value.Backend}";
        }
    }
}
