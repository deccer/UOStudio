using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ned.Client.Engine.UI;
using Serilog;
using Num = System.Numerics;

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
        private Texture2D _splashScreenTexture;
        private IntPtr _splashScreenTextureId;
        private bool _showSplashScreen = true;

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
            _logger.Information("Initializing...");
            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();

            _guiInputHandler = new ImGuiInputHandler();
            _guiRenderer = new ImGuiRenderer(this, _guiInputHandler)
                .Initialize()
                .RebuildFontAtlas();

            ImGui.GetIO().ConfigFlags = ImGuiConfigFlags.DockingEnable;
            base.Initialize();
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

        protected override void LoadContent()
        {
            _logger.Information("Loading Content...");
            base.LoadContent();

            _splashScreenTexture = Content.Load<Texture2D>("Content/splashscreen");
            _splashScreenTextureId = _guiRenderer.BindTexture(_splashScreenTexture);

            _logger.Information("Loading Content...Done");
        }

        protected override void UnloadContent()
        {
            _logger.Information("Unloading Content...");
            _splashScreenTexture.Dispose();
            base.UnloadContent();
            _logger.Information("Unloading Content...Done");
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

            if (_showSplashScreen && ImGui.Begin("Splashscreen"))
            {
                ImGui.SetWindowPos(new Num.Vector2(_graphics.PreferredBackBufferWidth / 2.0f - _splashScreenTexture.Width / 2.0f, _graphics.PreferredBackBufferHeight / 2.0f - _splashScreenTexture.Height / 2.0f));
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Num.Vector2(0, 0));
                if (ImGui.ImageButton(_splashScreenTextureId, new Num.Vector2(_splashScreenTexture.Width, _splashScreenTexture.Height), Num.Vector2.Zero, Num.Vector2.One, 0))
                {
                    _showSplashScreen = false;
                }
                ImGui.PopStyleVar();
                ImGui.End();
            }
        }
    }
}
