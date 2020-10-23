using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;
using UOStudio.Client.Core;
using UOStudio.Client.Core.Settings;
using UOStudio.Client.Engine.UI;
using UOStudio.Client.Engine.Ultima;
using UOStudio.Client.Network;
using Num = System.Numerics;

namespace UOStudio.Client.Engine
{
    public class ClientGame : Game
    {
        private readonly ILogger _logger;
        private readonly IAppSettingsProvider _appSettingsProvider;
        private readonly INetworkClient _networkClient;
        private readonly IUltimaProvider _ultimaProvider;
        private GraphicsDeviceManager _graphics;
        private ImGuiInputHandler _guiInputHandler;
        private ImGuiRenderer _guiRenderer;

        private KeyboardState _currentKeyboardState;
        private MouseState _currentMouseState;

        private readonly Color _clearColor = new Color(0.1f, 0.1f, 0.1f);
        private Texture2D _splashScreenTexture;
        private IntPtr _splashScreenTextureId;
        private bool _showSplashScreen = true;
        private bool _showLoginScreen = false;

        private string _serverName = "localhost";
        private string _serverPortText = "9050";
        private bool _showChat = true;
        private string _chatMessages = "";
        private string _sendChatMessage = "";
        private bool _showStatics;
        private bool _showLandTiles;

        private readonly IDictionary<UltimaTexture, IntPtr> _staticsTexturesMap;

        public ClientGame(ILogger logger, IAppSettingsProvider appSettingsProvider, INetworkClient networkClient, IUltimaProvider ultimaProvider)
        {
            _logger = logger;
            _appSettingsProvider = appSettingsProvider;
            _networkClient = networkClient;
            _ultimaProvider = ultimaProvider;

            _appSettingsProvider.Load();
            _networkClient.Connected += NetworkClientConnectedHandler;

            Window.Title = "UOStudio";

            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = _appSettingsProvider.AppSettings.Video.Width,
                PreferredBackBufferHeight = _appSettingsProvider.AppSettings.Video.Height,
                PreferMultiSampling = true,
                GraphicsProfile = GraphicsProfile.HiDef
            };
            _graphics.ApplyChanges();

            _staticsTexturesMap = new Dictionary<UltimaTexture, IntPtr>(0x8000);

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

            /*
            var loadUltimaAssetsTask = _ultimaProvider.Load(_logger, GraphicsDevice);
            Task.WaitAll(loadUltimaAssetsTask);

            for (uint staticIndex = 0; staticIndex < _ultimaProvider.ArtLoader.Entries.Length; ++staticIndex)
            {
                var staticTexture = _ultimaProvider.ArtLoader.GetTexture(staticIndex);
                if (staticTexture == null)
                {
                    continue;
                }

                var textureHandle = _guiRenderer.BindTexture(staticTexture);
                _staticsTexturesMap.Add(staticTexture, textureHandle);
            }
            */

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

        private void NetworkClientConnectedHandler(EndPoint endPoint, int clientId)
        {
            _showLoginScreen = false;
            _networkClient.SendMessage("Tadaaaa!");
        }

        private bool _stretchStaticTextures = true;

        private void DrawUi()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("File"))
                    {
                        if (!_networkClient.IsConnected && ImGui.MenuItem("Connect..."))
                        {
                            _showLoginScreen = true;
                        }

                        if (_networkClient.IsConnected && ImGui.MenuItem("Disconnect"))
                        {
                            _networkClient.Disconnect();
                        }

                        ImGui.Separator();
                        if (ImGui.MenuItem("Quit"))
                        {
                            _networkClient.Disconnect();

                            Exit();
                        }

                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Edit"))
                    {
                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("View"))
                    {
                        if (_networkClient.IsConnected)
                        {
                            ImGui.Checkbox("Chat", ref _showChat);
                            ImGui.Separator();
                        }

                        ImGui.Checkbox("Statics", ref _showStatics);
                        ImGui.Checkbox("Land Tiles", ref _showLandTiles);
                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Extras"))
                    {
                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Help"))
                    {
                        if (ImGui.MenuItem("About"))
                        {
                            if (ImGui.BeginPopup("UO Studio"))
                            {
                                ImGui.Text("Habla Blablah");
                                ImGui.EndPopup();
                            }
                        }
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
                    //_showLoginScreen = true;
                }
                ImGui.PopStyleVar();
                ImGui.End();
            }

            if (_showLoginScreen && ImGui.Begin("Login", ImGuiWindowFlags.NoCollapse))
            {
                ImGui.InputText("Server", ref _serverName, 64);
                ImGui.InputText("Port", ref _serverPortText, 5);

                if (ImGui.Button("Connect"))
                {
                    var serverPort = int.TryParse(_serverPortText, out var port) ? port : 0;

                    var profile = new Profile
                    {
                        ServerName = "localhost",
                        ServerPort = serverPort,
                        AccountName = "deccer",
                        AccountPassword = "password"
                    };
                    _networkClient.Connect(profile);
                }

                ImGui.End();
            }

            if (_showChat && ImGui.Begin("Chat"))
            {
                var currentItem = 0;
                ImGui.Text(_chatMessages);
                ImGui.ListBox("Users", ref currentItem, new[] { "User1", "User2" }, 2);
                ImGui.InputText("Message: ", ref _sendChatMessage, 128);
                ImGui.End();
            }

            if (_showStatics && ImGui.Begin("Statics"))
            {
                var windowViewport = ImGui.GetWindowViewport();
                var windowSize = ImGui.GetWindowSize();
                var windowPos = ImGui.GetWindowPos();
                ImGui.Checkbox("Stretch", ref _stretchStaticTextures);

                var drawList = ImGui.GetWindowDrawList();
                var perRowIndex = 0;
                ImGui.PushClipRect(windowPos, windowSize, true);
                foreach (var staticTexture in _staticsTexturesMap)
                {
                    if (perRowIndex == (int)(windowViewport.Size.X / 88))
                    {
                        perRowIndex = 0;
                    }

                    if (_stretchStaticTextures)
                    {
                        if (ImGui.ImageButton(staticTexture.Value, new Num.Vector2(88, 166)))
                        {
                        }
                    }
                    else
                    {
                        if (ImGui.ImageButton(staticTexture.Value, new Num.Vector2(staticTexture.Key.Width, staticTexture.Key.Height)))
                        {
                        }
                    }

                    if (perRowIndex > 0)
                    {
                        ImGui.SameLine();
                    }

                    perRowIndex++;
                }

                ImGui.PopClipRect();
                ImGui.End();
            }

            if (_showLandTiles && ImGui.Begin("Land Tiles"))
            {
                ImGui.End();
            }

            ImGui.ShowDemoWindow(ref _showLandTiles);
        }
    }
}
