using System;
using System.Net;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;
using UOStudio.Client.Core;
using UOStudio.Client.Core.Settings;
using UOStudio.Client.Engine.Resources;
using UOStudio.Client.Engine.UI;
using UOStudio.Client.Engine.Windows;
using UOStudio.Client.Network;
using UOStudio.Client.Resources;

namespace UOStudio.Client.Engine
{
    public enum UiStyle
    {
        Light,
        Dark,
        Discord,
        DiscordDarker,
        DiscordDark,
        Cherry,
        Red
    }

    public class ClientGame : Game
    {
        private readonly ILogger _logger;
        private readonly IAppSettingsProvider _appSettingsProvider;
        private readonly IFileVersionProvider _fileVersionProvider;
        private readonly ProfileService _profileService;
        private readonly INetworkClient _networkClient;
        private readonly GraphicsDeviceManager _graphics;
        private ImGuiRenderer _guiRenderer;

        private KeyboardState _currentKeyboardState;
        private MouseState _currentMouseState;

        private readonly Color _clearColor = new Color(0.1f, 0.1f, 0.1f);

        private bool _showChat = true;
        private bool _showStyleEditor = false;
        private bool _showDemoWindow = false;

        private ItemProvider _itemProvider;
        private TileDataProvider _tileDataProvider;

        private ProjectType _projectType;
        private GumpEditProjectWindowProvider _gumpEditProjectWindowProvider;
        private MapEditProjectWindowProvider _mapEditProjectWindowProvider;

        private RenderTarget2D _mapEditRenderTarget;
        private MapEditState _mapEditState;

        private UiStyle _uiStyle = UiStyle.Light;

        public ClientGame(
            ILogger logger,
            IAppSettingsProvider appSettingsProvider,
            IFileVersionProvider fileVersionProvider,
            ProfileService profileService,
            INetworkClient networkClient
        )
        {
            _logger = logger;
            _appSettingsProvider = appSettingsProvider;
            _fileVersionProvider = fileVersionProvider;
            _profileService = profileService;
            _networkClient = networkClient;

            _appSettingsProvider.Load();
            _networkClient.Connected += NetworkClientConnectedHandler;

            Window.Title = "UOStudio";
            Window.AllowUserResizing = true;

            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = _appSettingsProvider.AppSettings.Video.Width,
                PreferredBackBufferHeight = _appSettingsProvider.AppSettings.Video.Height,
                PreferMultiSampling = true,
                GraphicsProfile = GraphicsProfile.HiDef
            };
            _graphics.ApplyChanges();

            _mapEditState = new MapEditState();
            _projectType = ProjectType.Map;

            // _windowProvider = new WindowProvider(_appSettingsProvider);
            // _windowProvider.LoginWindow.OnConnect += (_, e) =>
            // {
            //     var profile = new Profile
            //     {
            //         ServerName = e.ServerName,
            //         ServerPort = e.ServerPort,
            //         AccountName = e.UserName,
            //         AccountPassword = e.Password
            //     };
            //     _networkClient.Connect(profile);
            // };
            // _windowProvider.LoginWindow.OnDisconnect += (_, __) =>
            // {
            //     _networkClient.Disconnect();
            // };

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _logger.Information("Initializing...");
            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();

            _guiRenderer = new ImGuiRenderer(this);
            ImGuiRenderer.EnableDocking();
            _guiRenderer.RebuildFontAtlas();

            ImGui.GetIO().ConfigFlags = ImGuiConfigFlags.DockingEnable;
            base.Initialize();
            _logger.Information("Initializing...Done");
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_mapEditRenderTarget);
            GraphicsDevice.Clear(Color.Purple);

            GraphicsDevice.SetRenderTarget(null);
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

            _mapEditRenderTarget = new RenderTarget2D(
                GraphicsDevice,
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8
            );

            _itemProvider = new ItemProvider(_logger, _appSettingsProvider.AppSettings.General.UltimaOnlineBasePath, false);
            _tileDataProvider = new TileDataProvider(_appSettingsProvider, false);

            _gumpEditProjectWindowProvider = new GumpEditProjectWindowProvider(_appSettingsProvider, _fileVersionProvider);
            _gumpEditProjectWindowProvider.LoadContent(GraphicsDevice, Content, _guiRenderer);
            _mapEditProjectWindowProvider = new MapEditProjectWindowProvider(
                _appSettingsProvider,
                _fileVersionProvider,
                _profileService,
                _itemProvider,
                _tileDataProvider,
                _mapEditState,
                _mapEditRenderTarget
            );
            _mapEditProjectWindowProvider.LoadContent(GraphicsDevice, Content, _guiRenderer);
            _mapEditProjectWindowProvider.MapConnectToServerWindow.OnConnect += LoginWindowOnOnConnect;
            _mapEditProjectWindowProvider.MapConnectToServerWindow.OnDisconnect += LoginWindowOnOnDisconnect;

            _logger.Information("Content - Loading...Done");
        }

        private void LoginWindowOnOnDisconnect(object sender, EventArgs e)
        {
            _networkClient.Disconnect();
        }

        private void LoginWindowOnOnConnect(object sender, ConnectEventArgs e)
        {
            var profile = new Profile
            {
                Name = "localhost",
                AccountName = "deccer",
                AccountPassword = "xxx",
                ServerName = "localhost",
                ServerPort = 9050
            };
            profile = _mapEditProjectWindowProvider.MapConnectToServerWindow.SelectedProfile;
            _networkClient.Connect(profile);
        }

        protected override void UnloadContent()
        {
            _logger.Information("Content - Unloading...");
            _mapEditProjectWindowProvider.UnloadContent();
            _gumpEditProjectWindowProvider.UnloadContent();
            base.UnloadContent();
            _logger.Information("Content - Unloading...Done");
        }

        protected override void Update(GameTime gameTime)
        {
            var previousKeyboardState = _currentKeyboardState;
            var previousMouseState = _currentMouseState;
            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();

            _guiRenderer.UpdateInput();
            base.Update(gameTime);
        }

        private void NetworkClientConnectedHandler(EndPoint endPoint, int clientId)
        {
            _networkClient.SendMessage("Tadaaaa!");
        }

        private void DrawUi()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu(ResGeneral.MenuFile))
                    {
                        if (ImGui.MenuItem(ResGeneral.MenuFileSettings))
                        {
                            if (_projectType == ProjectType.Map)
                            {
                                _mapEditProjectWindowProvider.Settings.Show();
                            }
                            else
                            {
                                _gumpEditProjectWindowProvider.Settings.Show();
                            }
                        }

                        ImGui.Separator();
                        if (ImGui.MenuItem(ResGeneral.MenuFileQuit))
                        {
                            _networkClient.Disconnect();

                            Exit();
                        }

                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu(ResGeneral.MenuMap))
                    {
                        if (ImGui.MenuItem(ResGeneral.MenuMapLocal))
                        {
                        }

                        if (ImGui.BeginMenu(ResGeneral.MenuMapRemote))
                        {
                            if (ImGui.MenuItem(ResGeneral.MenuMapRemoteConnect))
                            {
                                _mapEditProjectWindowProvider.MapConnectToServerWindow.Show();
                            }

                            if (_networkClient.IsConnected && ImGui.MenuItem(ResGeneral.MenuMapRemoteDisconnect))
                            {
                                _networkClient.Disconnect();
                            }

                            ImGui.EndMenu();
                        }

                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu(ResGeneral.MenuEdit))
                    {
                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu(ResGeneral.MenuView))
                    {
                        if (_networkClient.IsConnected)
                        {
                            ImGui.Checkbox("Chat", ref _showChat);
                            ImGui.Separator();
                        }

                        var showToolbarWindow = _mapEditProjectWindowProvider.MapToolbarWindow.IsVisible;
                        if (ImGui.Checkbox("Tools", ref showToolbarWindow))
                        {
                            _mapEditProjectWindowProvider.MapToolbarWindow.ToggleVisibility();
                        }

                        var showItemsWindow = _mapEditProjectWindowProvider.MapItemBrowserWindow.IsVisible;
                        if (ImGui.Checkbox("Items", ref showItemsWindow))
                        {
                            _mapEditProjectWindowProvider.MapItemBrowserWindow.ToggleVisibility();
                        }

                        var showLandsWindow = _mapEditProjectWindowProvider.MapLandBrowserWindow.IsVisible;
                        if (ImGui.Checkbox("Land Tiles", ref showLandsWindow))
                        {
                            _mapEditProjectWindowProvider.MapLandBrowserWindow.ToggleVisibility();
                        }

                        var showItemDetail = _mapEditProjectWindowProvider.MapTileDetailWindow.IsVisible;
                        if (ImGui.Checkbox("Details", ref showItemDetail))
                        {
                            _mapEditProjectWindowProvider.MapTileDetailWindow.ToggleVisibility();
                        }

                        if (ImGui.Checkbox("Chat", ref _showChat))
                        {
                        }

                        ImGui.Separator();

                        var styleSelected = (int)_uiStyle;
                        if (ImGui.RadioButton("Light", ref styleSelected, 0))
                        {
                            _uiStyle = UiStyle.Light;
                            ImGuiRenderer.SetStyle(_uiStyle);
                        }

                        if (ImGui.RadioButton("Dark", ref styleSelected, 1))
                        {
                            _uiStyle = UiStyle.Dark;
                            ImGuiRenderer.SetStyle(_uiStyle);
                        }

                        if (ImGui.RadioButton("Discord", ref styleSelected, 2))
                        {
                            _uiStyle = UiStyle.Discord;
                            ImGuiRenderer.SetStyle(_uiStyle);
                        }

                        if (ImGui.RadioButton("Discord Darker", ref styleSelected, 3))
                        {
                            _uiStyle = UiStyle.DiscordDarker;
                            ImGuiRenderer.SetStyle(_uiStyle);
                        }

                        if (ImGui.RadioButton("Discord Dark", ref styleSelected, 4))
                        {
                            _uiStyle = UiStyle.DiscordDark;
                            ImGuiRenderer.SetStyle(_uiStyle);
                        }

                        if (ImGui.RadioButton("Cherry", ref styleSelected, 5))
                        {
                            _uiStyle = UiStyle.Cherry;
                            ImGuiRenderer.SetStyle(_uiStyle);
                        }

                        if (ImGui.RadioButton("Red", ref styleSelected, 6))
                        {
                            _uiStyle = UiStyle.Red;
                            ImGuiRenderer.SetStyle(_uiStyle);
                        }

                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu(ResGeneral.MenuHelp))
                    {
                        if (ImGui.MenuItem(ResGeneral.MenuHelpAbout))
                        {
                            if (_projectType == ProjectType.Map)
                            {
                                _mapEditProjectWindowProvider.AboutWindow.Show();
                            }
                            else
                            {
                                _gumpEditProjectWindowProvider.AboutWindow.Show();
                            }
                        }

                        if (ImGui.MenuItem("Style Editor"))
                        {
                            _showStyleEditor = !_showStyleEditor;
                        }

                        if (ImGui.MenuItem("Imgui Demo"))
                        {
                            _showDemoWindow = !_showDemoWindow;
                        }

                        ImGui.EndMenu();
                    }

                    ImGui.EndMenuBar();
                }

                ImGui.EndMainMenuBar();
            }

            if (_projectType == ProjectType.Map)
            {
                _mapEditProjectWindowProvider.Draw();
            }
            else
            {
                _gumpEditProjectWindowProvider.Draw();
            }

            if (_showDemoWindow)
            {
                ImGui.ShowDemoWindow();
            }
            if (_showStyleEditor && ImGui.Begin("Edit Style"))
            {
                var currentStyle = ImGui.GetStyle();
                ImGui.ShowStyleEditor(currentStyle);
                ImGui.End();
            }

            if (_showChat && ImGui.Begin("Chat"))
            {
                ImGui.End();
            }
        }
    }
}
