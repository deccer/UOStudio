using System;
using System.Collections.Generic;
using System.Net;
using System.Resources;
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
using Num = System.Numerics;

namespace UOStudio.Client.Engine
{
    public class ClientGame : Game
    {
        private readonly ILogger _logger;
        private readonly IAppSettingsProvider _appSettingsProvider;
        private readonly IFileVersionProvider _fileVersionProvider;
        private readonly INetworkClient _networkClient;
        private readonly GraphicsDeviceManager _graphics;
        private ImGuiRenderer _guiRenderer;

        private KeyboardState _currentKeyboardState;
        private MouseState _currentMouseState;

        private readonly Color _clearColor = new Color(0.1f, 0.1f, 0.1f);

        private bool _showChat = true;
        private string _chatMessages = "";
        private string _sendChatMessage = "";
        private bool _showStatics;
        private bool _showLandTiles;
        private int _selectedStatic;
        private ItemData _selectedItemData;
        private int _selectedLandTile;
        private LandData _selectedLandData;
        private bool _stretchStaticTextures = true;
        private bool _stretchLandTextures = true;
        private bool _isLandSelected = false;
        private readonly Dictionary<IntPtr, int> _staticsIdMap;
        private readonly Dictionary<IntPtr, int> _landIdMap;

        private ItemProvider _itemProvider;
        private TileDataProvider _tileDataProvider;

        private readonly IDictionary<Texture2D, IntPtr> _staticsTexturesMap;
        private readonly IDictionary<Texture2D, IntPtr> _landTexturesMap;

        private ProjectType _projectType;
        private GumpEditProjectWindowProvider _gumpEditProjectWindowProvider;
        private MapEditProjectWindowProvider _mapEditProjectWindowProvider;

        private RenderTarget2D _mapEditRenderTarget;
        private MapEditState _mapEditState;

        public ClientGame(
            ILogger logger,
            IAppSettingsProvider appSettingsProvider,
            IFileVersionProvider fileVersionProvider,
            INetworkClient networkClient)
        {
            _logger = logger;
            _appSettingsProvider = appSettingsProvider;
            _fileVersionProvider = fileVersionProvider;
            _networkClient = networkClient;

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

            _staticsTexturesMap = new Dictionary<Texture2D, IntPtr>(0x8000);
            _landTexturesMap = new Dictionary<Texture2D, IntPtr>(0x8000);
            _staticsIdMap = new Dictionary<IntPtr, int>(8192);
            _landIdMap = new Dictionary<IntPtr, int>(8192);

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
            _guiRenderer.EnableDocking();
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
                DepthFormat.Depth24Stencil8);

            _gumpEditProjectWindowProvider = new GumpEditProjectWindowProvider(_appSettingsProvider, _fileVersionProvider);
            _gumpEditProjectWindowProvider.LoadContent(Content, _guiRenderer);
            _mapEditProjectWindowProvider = new MapEditProjectWindowProvider(
                _appSettingsProvider,
                _fileVersionProvider,
                _mapEditState,
                _mapEditRenderTarget
            );
            _mapEditProjectWindowProvider.LoadContent(Content, _guiRenderer);

            _itemProvider = new ItemProvider(_logger, _appSettingsProvider.AppSettings.General.UltimaOnlineBasePath, false);

            // for (var staticIndex = 0; staticIndex < _itemProvider.Length; ++staticIndex)
            // {
            //     var staticTexture = _itemProvider.GetStatic(GraphicsDevice, staticIndex);
            //     if (staticTexture == null)
            //     {
            //         continue;
            //     }
            //
            //     var textureHandle = _guiRenderer.BindTexture((Texture2D)staticTexture);
            //     _staticsTexturesMap.Add((Texture2D)staticTexture, textureHandle);
            //     _staticsIdMap.Add(textureHandle, (int)staticIndex);
            // }
            //
            // for (var landIndex = 0; landIndex < _itemProvider.Length; ++landIndex)
            // {
            //     var landTexture = _itemProvider.GetLand(GraphicsDevice, landIndex);
            //     if (landTexture == null)
            //     {
            //         continue;
            //     }
            //
            //     var textureHandle = _guiRenderer.BindTexture((Texture2D)landTexture);
            //     _landTexturesMap.Add((Texture2D)landTexture, textureHandle);
            //     _landIdMap.Add(textureHandle, (int)landIndex);
            // }

            _tileDataProvider = new TileDataProvider(_appSettingsProvider, false);

            _logger.Information("Content - Loading...Done");
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
                            if (!_networkClient.IsConnected && ImGui.MenuItem(ResGeneral.MenuMapRemoteConnect))
                            {
                                //_windowProvider.LoginWindow.Show();
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

                        ImGui.Checkbox("Statics", ref _showStatics);
                        ImGui.Checkbox("Land Tiles", ref _showLandTiles);
                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu(ResGeneral.MenuExtras))
                    {
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
                var windowSize = ImGui.GetWindowSize();

                ImGui.Checkbox("Stretch", ref _stretchStaticTextures);

                ImGui.BeginGroup();
                ImGui.InputInt("Static Id", ref _selectedStatic);
                ImGui.EndGroup();

                ImGui.BeginGroup();
                var perRowIndex = 0;
                foreach (var staticTexture in _staticsTexturesMap)
                {
                    if (perRowIndex == (int)(windowSize.X / (_stretchStaticTextures ? 88 : 44)) + 44)
                    {
                        perRowIndex = 0;
                    }

                    if (_stretchStaticTextures)
                    {
                        if (ImGui.ImageButton(staticTexture.Value, new Num.Vector2(88, 166)))
                        {
                            _selectedStatic = _staticsIdMap[staticTexture.Value];
                            _selectedItemData = _tileDataProvider.ItemTable[_selectedStatic];
                            _isLandSelected = false;
                        }
                    }
                    else
                    {
                        if (ImGui.ImageButton(staticTexture.Value, new Num.Vector2(staticTexture.Key.Width, staticTexture.Key.Height)))
                        {
                            _selectedStatic = _staticsIdMap[staticTexture.Value];
                            _selectedItemData = _tileDataProvider.ItemTable[_selectedStatic];
                            _isLandSelected = false;
                        }
                    }

                    if (perRowIndex > 0)
                    {
                        ImGui.SameLine();
                    }

                    perRowIndex++;
                }

                ImGui.EndGroup();
                ImGui.End();
            }

            if (_showLandTiles && ImGui.Begin("Land Tiles"))
            {
                var windowSize = ImGui.GetWindowSize();

                ImGui.Checkbox("Stretch", ref _stretchLandTextures);

                ImGui.BeginGroup();
                ImGui.InputInt("Land Id", ref _selectedLandTile);
                ImGui.EndGroup();

                ImGui.BeginGroup();
                var perRowIndex = 0;
                foreach (var landTexture in _landTexturesMap)
                {
                    if (perRowIndex == (int)(windowSize.X / (_stretchLandTextures ? 88 : 44)) + 1)
                    {
                        perRowIndex = 0;
                    }

                    if (_stretchLandTextures)
                    {
                        if (ImGui.ImageButton(landTexture.Value, new Num.Vector2(88, 166)))
                        {
                            _selectedLandTile = _landIdMap[landTexture.Value];
                            _selectedLandData = _tileDataProvider.LandTable[_selectedLandTile];
                            _isLandSelected = true;
                        }
                    }
                    else
                    {
                        if (ImGui.ImageButton(landTexture.Value, new Num.Vector2(landTexture.Key.Width, landTexture.Key.Height)))
                        {
                            _selectedLandTile = _landIdMap[landTexture.Value];
                            _selectedLandData = _tileDataProvider.LandTable[_selectedLandTile];
                            _isLandSelected = true;
                        }
                    }

                    if (perRowIndex > 0)
                    {
                        ImGui.SameLine();
                    }

                    perRowIndex++;
                }

                ImGui.EndGroup();
                ImGui.End();
            }

            if (ImGui.Begin("Item Details"))
            {
                ImGui.Columns(2);
                if (_isLandSelected)
                {
                    if (!string.IsNullOrEmpty(_selectedLandData.Name))
                    {
                        ImGui.TextUnformatted(_selectedLandData.Name);
                    }

                    ImGui.NextColumn();
                    ImGui.TextUnformatted(TileDataProvider.LandFlagsToString(_selectedLandData));
                }
                else
                {
                    if (!string.IsNullOrEmpty(_selectedItemData.Name))
                    {
                        ImGui.TextUnformatted(_selectedItemData.Name);
                    }

                    foreach (var property in TileDataProvider.ItemFlagsToPropertyList(_selectedItemData))
                    {
                        ImGui.TextUnformatted(property);
                        ImGui.NextColumn();
                    }
                }

                ImGui.End();
            }
        }
    }
}
