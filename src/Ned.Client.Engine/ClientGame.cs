using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ned.Client.Engine.UI;
using Serilog;

namespace Ned.Client.Engine
{
    public class ClientGame : Game
    {
        private readonly ILogger _logger;
        private GraphicsDeviceManager _graphics;
        private ImGuiInputHandler _guiInputHandler;
        private ImGuiRenderer _guiRenderer;

        private KeyboardState _currentKeyboardState;
        private MouseState _currentMouseState;

        private readonly Color _clearColor = new Color(0.1f, 0.1f, 0.1f);

        public ClientGame(ILogger logger)
        {
            _logger = logger;

            Window.Title = "NCentrED";

            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080,
                PreferMultiSampling = true,
                GraphicsProfile = GraphicsProfile.HiDef
            };
            _graphics.ApplyChanges();

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();

            _logger.Information("Initializing...");
            _guiInputHandler = new ImGuiInputHandler();
            _guiRenderer = new ImGuiRenderer(this, _guiInputHandler)
                .Initialize()
                .RebuildFontAtlas();

            ImGui.GetIO().ConfigFlags = ImGuiConfigFlags.DockingEnable;

            _logger.Information("Initializing...Done");
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_clearColor);

            _guiRenderer.BeginLayout(gameTime);
            DrawUi();
            _guiRenderer.EndLayout();
            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var previousKeyboardState = _currentKeyboardState;
            var previousMouseState = _currentMouseState;
            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();

            _guiInputHandler.Update(GraphicsDevice, ref _currentKeyboardState, ref _currentMouseState);
            base.Update(gameTime);
        }

        private void DrawUi()
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

                    if (ImGui.BeginMenu("Edit"))
                    {
                        ImGui.EndMenu();
                    }

                    ImGui.EndMenuBar();
                }

                ImGui.EndMainMenuBar();
            }
        }
    }
}
