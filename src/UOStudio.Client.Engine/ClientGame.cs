using System;
using System.Net;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;
using UOStudio.Client.Engine.UI;
using UOStudio.Client.Network;
using Num = System.Numerics;

namespace UOStudio.Client.Engine
{
    public class ClientGame : Game
    {
        private readonly ILogger _logger;
        private readonly INedClient _nedClient;
        private GraphicsDeviceManager _graphics;
        private ImGuiInputHandler _guiInputHandler;
        private ImGuiRenderer _guiRenderer;

        private KeyboardState _currentKeyboardState;
        private MouseState _currentMouseState;

        private readonly Color _clearColor = new Color(0.1f, 0.1f, 0.1f);
        private Texture2D _splashScreenTexture;
        private IntPtr _splashScreenTextureId;
        private bool _showSplashScreen = true;
        private bool _showLoginScreen;

        private string _serverName = "localhost";
        private string _serverPortText = "9050";
        private int _serverPort;

        public ClientGame(ILogger logger, INedClient nedClient)
        {
            _logger = logger;
            _nedClient = nedClient;
            _nedClient.Connected += NedClientConnectedHandler;

            Window.Title = "UOStudio";

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
            _logger.Information("Content - Loading...");
            base.LoadContent();

            _splashScreenTexture = Content.Load<Texture2D>("Content/splashscreen");
            _splashScreenTextureId = _guiRenderer.BindTexture(_splashScreenTexture);

            _logger.Information("Content - Loading...Done");
        }

        protected override void UnloadContent()
        {
            _logger.Information("Content - Unloading...");
            _splashScreenTexture.Dispose();
            base.UnloadContent();
            _logger.Information("Content - Unloading...Done");
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

        private void NedClientConnectedHandler(EndPoint endPoint, int clientId)
        {
            _showLoginScreen = false;
            _nedClient.SendMessage("Tadaaaa!");
        }

        private void DrawUi()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("File"))
                    {
                        if (ImGui.MenuItem("Connect..."))
                        {
                            _showLoginScreen = true;
                        }

                        if (ImGui.MenuItem("Disconnect"))
                        {
                            _nedClient.Disconnect();
                        }

                        ImGui.Separator();
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
                    _showLoginScreen = true;
                }
                ImGui.PopStyleVar();
                ImGui.End();
            }

            if (_showLoginScreen && ImGui.Begin("Login", ImGuiWindowFlags.NoCollapse))
            {
                ImGui.InputText("Server Name or Ip Address", ref _serverName, 64);
                ImGui.InputText("Port", ref _serverPortText, 5);

                if (ImGui.Button("Connect"))
                {
                    _serverPort = int.TryParse(_serverPortText, out var port) ? port : 0;
                    _nedClient.Connect(_serverName, _serverPort);
                }

                ImGui.End();
            }
        }
    }
}
