using System;
using System.Diagnostics;
using ImGuiNET;
using LiteNetLib;
using Serilog;
using UOStudio.Client.Engine;
using UOStudio.Client.Engine.Graphics;
using UOStudio.Client.Engine.Input;
using UOStudio.Client.Engine.Mathematics;
using UOStudio.Client.Engine.UI;
using UOStudio.Client.Services;
using UOStudio.Client.UI;
using UOStudio.Client.Worlds;
using UOStudio.Common.Contracts;
using UOStudio.Common.Network;
using Num = System.Numerics;

namespace UOStudio.Client
{
    internal sealed class MainGame : Application
    {
        private readonly Color _clearColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);
        private readonly ILogger _logger;
        private readonly IGraphicsDevice _graphicsDevice;
        private ImGuiController _imGuiController;

        private readonly ClientStartParameters _clientStartParameters;

        private readonly INetworkClient _networkClient;
        private readonly IWindowProvider _windowProvider;
        private readonly IWorldProvider _worldProvider;
        private readonly IContext _context;
        private readonly IWorldRenderer _worldRenderer;

        private readonly Camera _camera;

        private World _currentWorld;
        private bool _isWindowFocused;

        public MainGame(
            ILogger logger,
            WindowSettings windowSettings,
            ContextSettings contextSettings,
            IGraphicsDevice graphicsDevice,
            IMeshLibrary meshLibrary,
            IMaterialLibrary materialLibrary,
            IShaderProgramLibrary shaderProgramLibrary,
            ClientStartParameters clientStartParameters,
            INetworkClient networkClient,
            IWindowProvider windowProvider,
            IWorldProvider worldProvider,
            IWorldRenderer worldRenderer,
            IContext context)
            : base(logger, windowSettings, contextSettings, graphicsDevice)
        {
            _logger = logger;
            _graphicsDevice = graphicsDevice;

            _clientStartParameters = clientStartParameters;

            _networkClient = networkClient;
            _networkClient.Connected += NetworkClientConnected;
            _networkClient.Disconnected += NetworkClientDisconnected;
            _networkClient.ChunkReceived += NetworkClientChunkReceived;

            _windowProvider = windowProvider;
            _context = context;
            _windowProvider.Load();

            _worldProvider = worldProvider;
            _worldRenderer = worldRenderer;

            _camera = new Camera(FrameWidth, FrameHeight, new Vector3(0, 0, 3), Vector3.Up);
        }

        protected override bool Load()
        {
            var sw = Stopwatch.StartNew();
            _logger.Information("App: Initializing...");

            _imGuiController = new ImGuiController(_graphicsDevice, ResolutionWidth, ResolutionHeight);

            _currentWorld = _worldProvider.GetWorld(_clientStartParameters.ProjectId);

            sw.Stop();
            _logger.Information("App: Initializing...Done. Took {@TotalSeconds}s", sw.Elapsed.TotalSeconds);

            _logger.Information("Content: Loading...");
            _worldRenderer.Load(_graphicsDevice);
            _logger.Information("Content: Loading...Done");

            _networkClient.Connect("localhost", 9050);

            return true;
        }

        protected override void Render(float elapsedTime, float deltaTime)
        {
            if (!_isWindowFocused)
            {
                return;
            }

            _graphicsDevice.Clear(_clearColor);

            _worldRenderer.Draw(_graphicsDevice, _currentWorld, _camera);


            RenderUserInterface();
        }

        protected override void Resized(int width, int height)
        {
            base.Resized(width, height);
            _graphicsDevice.SetViewport(0, 0, FrameWidth, FrameHeight);
            _camera.Resize(FrameWidth, FrameHeight);

            _imGuiController?.Dispose();
            _imGuiController?.WindowResized(ResolutionWidth, ResolutionHeight);
            _imGuiController = new ImGuiController(_graphicsDevice, ResolutionWidth, ResolutionHeight);
        }

        protected override void Unload()
        {
            _networkClient.Disconnect();

            _logger.Information("Content: Unloading...");
            _imGuiController?.Dispose();
            _logger.Information("Content: Unloading...Done");
            base.Unload();
        }

        protected override void Update(float elapsedTime, float deltaTime)
        {
            _networkClient.Update();

            if (!_isWindowFocused)
            {
                return;
            }

            _imGuiController.Update(deltaTime);

            if (Keyboard.GetKey(Keys.Escape))
            {
                Close();
            }

            var movement = Vector3.Zero;
            var speedFactor = 200f;
            if (Keyboard.GetKey(Keys.W))
            {
                movement += _camera.Direction;
            }

            if (Keyboard.GetKey(Keys.S))
            {
                movement -= _camera.Direction;
            }

            if (Keyboard.GetKey(Keys.A))
            {
                movement -= _camera.Right;
            }

            if (Keyboard.GetKey(Keys.D))
            {
                movement += _camera.Right;
            }

            if (Keyboard.GetKey(Keys.Escape))
            {
                Close();
            }

            if (Keyboard.GetKey(Keys.F1))
            {
                _camera.Position = new Vector3(0, 0, 8);
            }

            movement = Vector3.Normalize(movement);
            if (Keyboard.GetKey(Keys.LeftShift))
            {
                movement *= speedFactor;
            }

            _camera.ProcessKeyboard(movement, deltaTime);
            var mouseState = Mouse.GetState();
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                _camera.ProcessMouseMovement();
            }
        }

        private void RenderUserInterface()
        {
            _imGuiController.BeginLayout();

            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("File"))
                    {
                        if (ImGui.MenuItem("Quit"))
                        {
                            Close();
                        }

                        ImGui.EndMenu();
                    }

                    ImGui.EndMenuBar();
                }

                ImGui.EndMainMenuBar();
            }

            ImGui.ShowDemoWindow();

            _windowProvider.Draw();
            _imGuiController.EndLayout();
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
        }
    }
}
