using System;
using System.Net;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;
using UOStudio.Client.Core;
using UOStudio.Client.Core.Settings;
using UOStudio.Client.Engine;
using UOStudio.Client.Engine.UI;
using UOStudio.Client.Engine.Windows;
using UOStudio.Client.Engine.Windows.General;
using UOStudio.Client.Engine.Windows.MapEdit;
using UOStudio.Client.Network;
using UOStudio.Client.Resources;

namespace UOStudio.Client
{
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

        private RenderTarget2D _mapEditRenderTarget;
        private MapEditState _mapEditState;

        private UiStyle _uiStyle = UiStyle.Light;

        // windows
        private DesktopWindow _desktopWindow;
        private AboutWindow _aboutWindow;
        private SplashScreenWindow _splashScreenWindow;
        private FrameTimeOverlayWindow _frameTimeOverlayWindow;
        private SettingsWindow _settingsWindow;

        private MapToolbarWindow _mapToolbarWindow;
        private MapViewWindow _mapViewWindow;
        private MapItemBrowserWindow _mapItemBrowserWindow;
        private MapLandBrowserWindow _mapLandBrowserWindow;
        private MapTileDetailWindow _mapTileDetailWindow;
        private MapTilePreviewWindow _mapTilePreviewWindow;
        private MapConnectToServerWindow _mapConnectToServerWindow;
        private MapListProjectsWindow _mapListProjectsWindow;

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

            _splashScreenWindow = new SplashScreenWindow(_fileVersionProvider);
            _aboutWindow = new AboutWindow(_fileVersionProvider);
            _frameTimeOverlayWindow = new FrameTimeOverlayWindow("Frame Times");
            _settingsWindow = new SettingsWindow(_appSettingsProvider);

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

            // general windows
            _desktopWindow = new DesktopWindow();
            _splashScreenWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);
            _aboutWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);
            _frameTimeOverlayWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);
            _settingsWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);

            // map edit windows
            _mapToolbarWindow = new MapToolbarWindow();
            _mapToolbarWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);
            var renderTargetId = _guiRenderer.BindTexture(_mapEditRenderTarget);
            _mapViewWindow = new MapViewWindow(_mapEditState, renderTargetId);
            _mapTileDetailWindow = new MapTileDetailWindow(_itemProvider);
            _mapTileDetailWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);
            _mapTilePreviewWindow = new MapTilePreviewWindow();
            _mapTilePreviewWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);
            _mapLandBrowserWindow = new MapLandBrowserWindow(_itemProvider, _tileDataProvider, _mapTileDetailWindow, _mapTilePreviewWindow);
            _mapLandBrowserWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);
            _mapItemBrowserWindow = new MapItemBrowserWindow(_itemProvider, _tileDataProvider, _mapTileDetailWindow, _mapTilePreviewWindow);
            _mapItemBrowserWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);
            _mapConnectToServerWindow = new MapConnectToServerWindow(_profileService);
            _mapConnectToServerWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);
            _mapListProjectsWindow = new MapListProjectsWindow();
            _mapListProjectsWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);

            _mapConnectToServerWindow.OnConnect += LoginWindowOnOnConnect;
            _mapConnectToServerWindow.OnDisconnect += LoginWindowOnOnDisconnect;

            _logger.Information("Content - Loading...Done");
        }

        private void LoginWindowOnOnDisconnect(object sender, EventArgs e)
        {
            _networkClient.Disconnect();
        }

        private void LoginWindowOnOnConnect(object sender, ConnectEventArgs e)
        {
            var profile = _mapConnectToServerWindow.SelectedProfile;
            _networkClient.Connect(profile);
        }

        protected override void UnloadContent()
        {
            _logger.Information("Content - Unloading...");
            // common windows
            _desktopWindow.UnloadContent();
            _splashScreenWindow.UnloadContent();
            _aboutWindow.UnloadContent();
            _frameTimeOverlayWindow.UnloadContent();
            _settingsWindow.UnloadContent();
            // map edit related windows
            _mapToolbarWindow.UnloadContent();
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
            _mapConnectToServerWindow.Hide();
            _mapListProjectsWindow.Show();
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
                                _settingsWindow.Show();
                            }
                            else
                            {
                                _settingsWindow.Show();
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
                                _mapConnectToServerWindow.Show();
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

                        var showToolbarWindow = _mapToolbarWindow.IsVisible;
                        if (ImGui.Checkbox("Tools", ref showToolbarWindow))
                        {
                            _mapToolbarWindow.ToggleVisibility();
                        }

                        var showItemsWindow = _mapItemBrowserWindow.IsVisible;
                        if (ImGui.Checkbox("Items", ref showItemsWindow))
                        {
                            _mapItemBrowserWindow.ToggleVisibility();
                        }

                        var showLandsWindow = _mapLandBrowserWindow.IsVisible;
                        if (ImGui.Checkbox("Land Tiles", ref showLandsWindow))
                        {
                            _mapLandBrowserWindow.ToggleVisibility();
                        }

                        var showItemDetail = _mapTileDetailWindow.IsVisible;
                        if (ImGui.Checkbox("Details", ref showItemDetail))
                        {
                            _mapTileDetailWindow.ToggleVisibility();
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
                                _aboutWindow.Show();
                            }
                            else
                            {
                                _aboutWindow.Show();
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

            _desktopWindow.Draw();
            _splashScreenWindow.Draw();
            _aboutWindow.Draw();
            _frameTimeOverlayWindow.Draw();
            _settingsWindow.Draw();
            if (_projectType == ProjectType.Map)
            {
                _mapToolbarWindow.Draw();
                _mapViewWindow.Draw();
                _mapItemBrowserWindow.Draw();
                _mapLandBrowserWindow.Draw();
                _mapTileDetailWindow.Draw();
                _mapTilePreviewWindow.Draw();
                _mapConnectToServerWindow.Draw();
                _mapListProjectsWindow.Draw();
            }
            else
            {
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
