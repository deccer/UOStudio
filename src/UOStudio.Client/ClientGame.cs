using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
using Vector2 = System.Numerics.Vector2;

namespace UOStudio.Client
{
    public readonly struct Tile
    {
        public Tile(int tileId, int z)
        {
            TileId = tileId;
            Z = z;
        }

        public int Z { get; }

        public int TileId { get; }
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
        private readonly Camera _camera;
        private TileBatcher _batcher;
        private Effect _batcherEffect;

        private KeyboardState _currentKeyboardState;
        private MouseState _currentMouseState;

        private readonly Color _clearColor = new Color(0.1f, 0.1f, 0.1f);

        private bool _showChat = true;
        private bool _showDemoWindow = false;

        private Map _map;

        private ItemProvider _itemProvider;
        private TileDataProvider _tileDataProvider;

        private ProjectType _projectType;

        private RenderTarget2D _mapEditRenderTarget;
        private EditorState _editorState = EditorState.Debug;

        private UiStyle _uiStyle = UiStyle.Light;

        // windows
        private DesktopWindow _desktopWindow;
        private AboutWindow _aboutWindow;
        private SplashScreenWindow _splashScreenWindow;
        private FrameTimeOverlayWindow _frameTimeOverlayWindow;
        private SettingsWindow _settingsWindow;
        private LogWindow _logWindow;
        private StyleEditorWindow _styleEditorWindow;
        private ChatWindow _chatWindow;

        private MapToolbarWindow _mapToolbarWindow;
        private MapViewWindow _mapViewWindow;
        private MapItemBrowserWindow _mapItemBrowserWindow;
        private MapLandBrowserWindow _mapLandBrowserWindow;
        private MapTileDetailWindow _mapTileDetailWindow;
        private MapTilePreviewWindow _mapTilePreviewWindow;
        private MapConnectToServerWindow _mapConnectToServerWindow;
        private MapSelectProjectsWindow _mapSelectProjectsWindow;
        private MapAddOrEditProfileWindow _mapAddOrEditProfileWindow;
        private MapViewProfileWindow _mapViewProfileWindow;
        private MapCreateProjectWindow _mapCreateProjectWindow;
        private double _networkClientDownloadPercentage;

        //private BasicEffect _mapEffect;
        //private VertexBuffer _mapVertexBuffer;
        private IList<VertexPositionColorTexture> _mapVertices;
        private RasterizerState _wireframeRasterizerState;
        private IDictionary<(int, int), Tile> _mapTiles;

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
            _networkClient.Disconnected += NetworkClientDisconnectedHandler;
            _networkClient.LoginSuccessful += NetworkClientOnLoginSuccessful;
            _networkClient.LoginFailed += NetworkClientOnLoginFailed;
            _networkClient.JoinProjectSuccessful += NetworkClientOnJoinProjectSuccessful;
            _networkClient.JoinProjectFailed += NetworkClientOnJoinProjectFailed;
            _networkClient.DownloadProgress += NetworkClientOnDownloadProgress;

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
            _camera = new Camera(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            //_editorState = EditorState.Disconnected;
            _projectType = ProjectType.Map;

            IsMouseVisible = true;
        }

        private void NetworkClientOnDownloadProgress(double percentage)
        {
            _networkClientDownloadPercentage = percentage;
        }

        protected override void Initialize()
        {
            _logger.Information("Initializing...");
            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();

            _guiRenderer = new ImGuiRenderer(this);
            _guiRenderer.RebuildFontAtlas();

            _splashScreenWindow = new SplashScreenWindow(_fileVersionProvider);
            _aboutWindow = new AboutWindow(_fileVersionProvider);
            _frameTimeOverlayWindow = new FrameTimeOverlayWindow("Frame Times");
            _settingsWindow = new SettingsWindow(_appSettingsProvider);
            _logWindow = new LogWindow();
            _styleEditorWindow = new StyleEditorWindow();
            _chatWindow = new ChatWindow();

            _mapVertices = new List<VertexPositionColorTexture>();

            base.Initialize();
            _logger.Information("Initializing...Done");
            _logWindow.AddLogMessage(LogType.Info, "Initializing...Done");
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_mapEditRenderTarget);
            GraphicsDevice.Viewport = new Viewport(_mapEditRenderTarget.Bounds);
            GraphicsDevice.Clear(Color.Purple);

            GraphicsDevice.SetStringMarkerEXT("Batcher Begin");
            _batcher.Begin();
            _map.Draw(_batcher, _dummyTexture);
            _batcher.End();
            GraphicsDevice.SetStringMarkerEXT("Batcher End");

            //_mapEffect.World = Matrix.Identity * Matrix.CreateScale(new Vector3(1, -1, 1));
            //_mapEffect.View = _camera.ViewMatrix;//Matrix.CreateLookAt(new Vector3(4, -4, 768), new Vector3(4, -4, 0), Vector3.Up);
            //_mapEffect.Projection = Matrix.CreateOrthographic(40, 22.5f, 0.05f, 64f);
            //_mapEffect.Projection = _camera.ProjectionMatrix;//Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _mapEditRenderTarget.Width / (float)_mapEditRenderTarget.Height, 0.1f, 2048.0f);

            //GraphicsDevice.SetVertexBuffer(_mapVertexBuffer);
            //GraphicsDevice.RasterizerState = _wireframeRasterizerState;
            //foreach (var pass in _mapEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, _mapVertices.Count / 3);
            //}

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Viewport = new Viewport(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            GraphicsDevice.Clear(_clearColor);

            GraphicsDevice.SetVertexBuffer(null);

            _guiRenderer.BeginLayout(gameTime);
            DrawUi();
            _guiRenderer.EndLayout();
            base.Draw(gameTime);
        }

        private Texture2D _dummyTexture;

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

            var heightMap = new sbyte []
            {
                30, 29, 27, 28, 18, 0, 0, 0,
                29, 25, 22, 19, 21, 0, 0, 0,
                27, 24, 0, 0, 0, 0, 0, 0,
                22, 21, 0, 0, 0, 0, 0, 0,
                20, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
            };
            _mapTiles = new Dictionary<(int, int), Tile>(64);
            for (var y = 0; y < 8; y++)
            {
                for (var x = 0; x < 8; x++)
                {
                    var z = heightMap[y * 8 + x];
                    _mapTiles.Add((x, y), new Tile(0, z));
                }
            }

            var textureData = new Color[44 * 44];
            _dummyTexture = new Texture2D(GraphicsDevice, 44, 44);

            for (var y = 0; y < 44; y++)
            {
                for (var x = 0; x < 44; x++)
                {
                    textureData[y * 44 + x] = Color.Red;
                }
            }
            _dummyTexture.SetData(textureData);

            _map = new Map(heightMap, 8, 8);
            _batcherEffect = Content.Load<Effect>("Content/Shaders/IsometricEffect.fxc");
            _batcher = new TileBatcher(GraphicsDevice, _batcherEffect);

            //_mapVertexBuffer?.Dispose();
            //_mapVertexBuffer = BuildVertexBuffer();
            //_mapEffect = new BasicEffect(GraphicsDevice);
            //_mapEffect.VertexColorEnabled = true;
            _wireframeRasterizerState = new RasterizerState();
            _wireframeRasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            _wireframeRasterizerState.FillMode = FillMode.WireFrame;
            _wireframeRasterizerState.MultiSampleAntiAlias = true;

            _itemProvider = new ItemProvider(_logger);
            _tileDataProvider = new TileDataProvider();

            // general windows
            _desktopWindow = new DesktopWindow();
            _splashScreenWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);
            _aboutWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);
            _frameTimeOverlayWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);
            _settingsWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);

            // map edit windows
            _mapToolbarWindow = new MapToolbarWindow();
            _mapToolbarWindow.LoginClicked += MapToolbarLoginClicked;
            _mapToolbarWindow.LogoutClicked += MapToolbarLogoutClicked;
            _mapToolbarWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);
            var renderTargetId = _guiRenderer.BindTexture(_mapEditRenderTarget);
            _mapViewWindow = new MapViewWindow(_editorState, renderTargetId, _mapEditRenderTarget.Width, _mapEditRenderTarget.Height);
            _mapViewWindow.Show();

            _mapTileDetailWindow = new MapTileDetailWindow();
            _mapTileDetailWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);

            _mapTilePreviewWindow = new MapTilePreviewWindow();
            _mapTilePreviewWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);

            _mapLandBrowserWindow = new MapLandBrowserWindow(_itemProvider, _tileDataProvider, _mapTileDetailWindow, _mapTilePreviewWindow);
            _mapLandBrowserWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);

            _mapItemBrowserWindow = new MapItemBrowserWindow(_itemProvider, _tileDataProvider, _mapTileDetailWindow, _mapTilePreviewWindow);
            _mapItemBrowserWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);

            _mapConnectToServerWindow = new MapConnectToServerWindow(_profileService);
            _mapConnectToServerWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);
            _mapConnectToServerWindow.ConnectClicked += ConnectToServerWindowConnectClicked;
            _mapConnectToServerWindow.DisconnectClicked += ConnectToServerWindowDisconnectClicked;
            _mapConnectToServerWindow.EditProfilesClicked += ConnectToServerEditProfilesClicked;

            _mapViewProfileWindow = new MapViewProfileWindow(_profileService);
            _mapViewProfileWindow.AddProfileClicked += MapViewProfileWindowOnAddProfileClicked;
            _mapViewProfileWindow.DeleteProfileClicked += MapViewProfileWindowOnDeleteProfileClicked;
            _mapViewProfileWindow.UpdateProfileClicked += MapViewProfileWindowOnUpdateProfileClicked;

            _mapAddOrEditProfileWindow = new MapAddOrEditProfileWindow(_profileService);

            _mapSelectProjectsWindow = new MapSelectProjectsWindow();
            _mapSelectProjectsWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);
            _mapSelectProjectsWindow.SelectProjectClicked += MapSelectProjectsWindowOnSelectProjectClicked;
            _mapSelectProjectsWindow.DeleteProjectClicked += MapSelectProjectsWindowOnDeleteProjectClicked;
            _mapSelectProjectsWindow.CreateProjectClicked += MapSelectProjectsWindowOnCreateProjectClicked;
            _mapSelectProjectsWindow.RefreshProjectListClicked += MapSelectProjectsWindowOnRefreshProjectListClicked;

            _mapCreateProjectWindow = new MapCreateProjectWindow();
            _mapCreateProjectWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);
            _mapCreateProjectWindow.CreateProjectClicked += MapCreateProjectWindowOnCreateProjectClicked;

            _logger.Information("Content - Loading...Done");
        }

        private float GetTileZ(int x, int y, float defaultZ) => _mapTiles.TryGetValue((x, y), out var tile)
            ? tile.Z
            : defaultZ;

        private VertexBuffer BuildVertexBuffer()
        {
            const int TileSize = 44;
            const int TileSizeHalf = TileSize / 2;
            /*
for (int y = 1; y < gridY+1; y++) {
    for (int x = 1; x < gridX+1; x++) {
        drawSquare((x-y) * 38 + (SCREEN_SIZE_X/2), (x+y) * 19);
    }
}

glTexCoord2f(0,1); glVertex2i(x+38, y);
glTexCoord2f(1,1); glVertex2i(x,    y+19);
glTexCoord2f(1,0); glVertex2i(x-38, y);
glTexCoord2f(0,0); glVertex2i(x,    y-19);
             */

            void DrawSquare(int x, int y, float tileZ, float tileZEast, float tileZWest, float tileZSouth)
            {
                var p0 = new Vector3(x + TileSizeHalf, y - tileZWest * 4, 0);
                var p1 = new Vector3(x + TileSize, y + TileSizeHalf - tileZSouth * 4, 0);
                var p2 = new Vector3(x, y + TileSizeHalf - tileZ, 0);
                var p3 = new Vector3(x + TileSizeHalf, y + TileSize - tileZEast * 4, 0);

                var n1 = Vector3.Cross(p2 - p0, p1 - p0);

                _mapVertices.Add(new VertexPositionColorTexture(p0, Color.White, Microsoft.Xna.Framework.Vector2.Zero));
                _mapVertices.Add(new VertexPositionColorTexture(p1, Color.White, Microsoft.Xna.Framework.Vector2.Zero));
                _mapVertices.Add(new VertexPositionColorTexture(p2, Color.White, Microsoft.Xna.Framework.Vector2.Zero));
                //_mapVertices.Add(new VertexPositionColorTexture(p3, Color.Black, Microsoft.Xna.Framework.Vector2.Zero));
                var n2 = Vector3.Cross(p2 - p3, p1 - p3);

                //_mapVertices.Add(new VertexPositionColorTexture(p1, Color.Black, Microsoft.Xna.Framework.Vector2.Zero));
                //_mapVertices.Add(new VertexPositionColorTexture(p3, Color.Black, Microsoft.Xna.Framework.Vector2.Zero));
                //_mapVertices.Add(new VertexPositionColorTexture(p2, Color.Black, Microsoft.Xna.Framework.Vector2.Zero));

                /*
                _mapVertices.Add(new VertexPositionColorTexture(new Vector3(x, y, tileZ * 4), Color.White, Microsoft.Xna.Framework.Vector2.Zero));
                _mapVertices.Add(new VertexPositionColorTexture(new Vector3(x - 22, y + 11 - (tileZWest * 4), 0), Color.White, Microsoft.Xna.Framework.Vector2.Zero));
                _mapVertices.Add(new VertexPositionColorTexture(new Vector3(x + 22, y + 11 - (tileZEast * 4), 0), Color.White, Microsoft.Xna.Framework.Vector2.Zero));

                _mapVertices.Add(new VertexPositionColorTexture(new Vector3(x - 22, y + 11 - (tileZWest * 4), 0), Color.DarkGray, Microsoft.Xna.Framework.Vector2.Zero));
                _mapVertices.Add(new VertexPositionColorTexture(new Vector3(x, y + 22 - (tileZSouth * 4), 0), Color.DarkGray, Microsoft.Xna.Framework.Vector2.Zero));
                _mapVertices.Add(new VertexPositionColorTexture(new Vector3(x + 22, y + 11 - (tileZEast * 4), 0), Color.DarkGray, Microsoft.Xna.Framework.Vector2.Zero));
                */
            }

            for (var x = 0; x < 8; ++x)
            {
                for (var y = 0; y < 8; ++y)
                {
                    var tileZ = GetTileZ(x, y, -15);
                    var tileZEast = GetTileZ(x + 1, y, tileZ);
                    var tileZWest = GetTileZ(x, y + 1, tileZ);
                    var tileZSouth = GetTileZ(x + 1, y + 1, tileZ);

                    DrawSquare((x - y) * TileSizeHalf, (x + y) * TileSizeHalf - 200, tileZ, tileZEast, tileZWest, tileZSouth);
                }
            }

            var vertexBuffer = new VertexBuffer(
                GraphicsDevice,
                typeof(VertexPositionColorTexture),
                _mapVertices.Count,
                BufferUsage.WriteOnly
            );
            vertexBuffer.SetData(_mapVertices.ToArray());
            return vertexBuffer;
            /*
      GetLandAlt(item.X, item.Y + 1, z, rawZ, west, rawWest);
      GetLandAlt(item.X + 1, item.Y + 1, z, rawZ, south, rawSouth);
      GetLandAlt(item.X + 1, item.Y, z, rawZ, east, rawEast);

      if  (west <> z) or (south <> z) or (east <> z) then
        ABlockInfo^.HighRes := FTextureManager.GetTexMaterial(item.TileID);

      if (rawWest <> rawZ) or (rawSouth <> rawZ) or (rawEast <> rawZ) then
      begin
        ABlockInfo^.RealQuad[0][0] := drawX;
        ABlockInfo^.RealQuad[0][1] := drawY - rawZ * 4;
        ABlockInfo^.RealQuad[1][0] := drawX + 22;
        ABlockInfo^.RealQuad[1][1] := drawY + 22 - rawEast * 4;
        ABlockInfo^.RealQuad[2][0] := drawX;
        ABlockInfo^.RealQuad[2][1] := drawY + 44 - rawSouth * 4;
        ABlockInfo^.RealQuad[3][0] := drawX - 22;
        ABlockInfo^.RealQuad[3][1] := drawY + 22 - rawWest * 4;
             */
        }

        private void MapViewProfileWindowOnUpdateProfileClicked(Profile profile)
        {
            _mapAddOrEditProfileWindow.AddOrEdit = ProfileAddOrEdit.Edit;
            _mapAddOrEditProfileWindow.SelectedProfile = profile;
            _mapAddOrEditProfileWindow.Show();
        }

        private void MapViewProfileWindowOnDeleteProfileClicked(Profile profile)
        {
            // TODO(deccer): add confirmation dialog
            if (profile != null)
            {
                _profileService.Remove(profile);
            }
        }

        private void MapViewProfileWindowOnAddProfileClicked()
        {
            _mapAddOrEditProfileWindow.AddOrEdit = ProfileAddOrEdit.Add;
            _mapAddOrEditProfileWindow.SelectedProfile = null;
            _mapAddOrEditProfileWindow.Show();
        }

        private async void MapCreateProjectWindowOnCreateProjectClicked()
        {
            var projectName = _mapCreateProjectWindow.Name;
            var projectDescription = _mapCreateProjectWindow.Description;
            var projectClientVersion = _mapCreateProjectWindow.ClientVersion;

            var projectId = await _networkClient.CreateProjectAsync(projectName, projectDescription, projectClientVersion);
            var projects = await _networkClient.GetProjectsAsync();
            _mapSelectProjectsWindow.SetProjects(projects);
            _mapCreateProjectWindow.Hide();
        }

        private async void MapSelectProjectsWindowOnRefreshProjectListClicked()
        {
            var projects = await _networkClient.GetProjectsAsync();
            _mapSelectProjectsWindow.SetProjects(projects);
        }

        private async void MapSelectProjectsWindowOnCreateProjectClicked()
        {
            _mapCreateProjectWindow.Show();
        }

        private void MapSelectProjectsWindowOnSelectProjectClicked(Project project)
        {
            _networkClient.JoinProject(project.Id);
        }

        private async void MapSelectProjectsWindowOnDeleteProjectClicked(Project project)
        {
            var result = await _networkClient.DeleteProjectAsync(project.Id);
        }

        private void MapToolbarLogoutClicked()
        {
        }

        private void MapToolbarLoginClicked()
        {
            _mapConnectToServerWindow.Show();
        }

        private void ConnectToServerWindowDisconnectClicked(object sender, EventArgs e)
        {
            _networkClient.Disconnect();
        }

        private void ConnectToServerWindowConnectClicked(object sender, ConnectEventArgs e)
        {
            var profile = _mapConnectToServerWindow.SelectedProfile;

            var encoded = Encoding.ASCII.GetBytes($"{profile.AccountName}:{profile.AccountPassword}");
            File.WriteAllText(Path.Combine(Path.GetTempPath(), "uostudio.uostudio"), Convert.ToBase64String(encoded));

            _networkClient.Connect(profile);
        }

        private void ConnectToServerEditProfilesClicked()
        {
            _mapViewProfileWindow.Show();
        }

        protected override void UnloadContent()
        {
            _logger.Information("Content - Unloading...");
            // common windows
            _desktopWindow?.UnloadContent();
            _splashScreenWindow?.UnloadContent();
            _aboutWindow?.UnloadContent();
            _frameTimeOverlayWindow?.UnloadContent();
            _settingsWindow?.UnloadContent();
            _chatWindow?.UnloadContent();
            _logWindow?.UnloadContent();
            // map edit related windows
            _mapToolbarWindow?.UnloadContent();
            _mapViewWindow?.UnloadContent();
            _mapTileDetailWindow?.UnloadContent();
            _mapTilePreviewWindow?.UnloadContent();
            _mapLandBrowserWindow?.UnloadContent();
            _mapItemBrowserWindow?.UnloadContent();
            _mapConnectToServerWindow?.UnloadContent();
            _mapViewProfileWindow?.UnloadContent();
            _mapSelectProjectsWindow?.UnloadContent();
            _mapCreateProjectWindow?.UnloadContent();
            _mapAddOrEditProfileWindow?.UnloadContent();

            base.UnloadContent();
            _logger.Information("Content - Unloading...Done");
        }

        protected override void Update(GameTime gameTime)
        {
            _networkClient.Update();
            var previousKeyboardState = _currentKeyboardState;
            var previousMouseState = _currentMouseState;
            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();

            _camera.Update(_mapEditRenderTarget.Width, _mapEditRenderTarget.Height);

            _guiRenderer.UpdateInput();
            base.Update(gameTime);
        }

        private void NetworkClientOnJoinProjectFailed(string reason)
        {
            _mapSelectProjectsWindow.Show();
        }

        private void NetworkClientOnJoinProjectSuccessful(string projectPath)
        {
            _mapSelectProjectsWindow.Hide();

            _desktopWindow.Show();
            _mapToolbarWindow.Show();
            _mapViewWindow.Show();
            _mapTileDetailWindow.Show();
            _mapTilePreviewWindow.Show();

            _itemProvider.Load(projectPath);
            _tileDataProvider.Load(projectPath, false);
            _mapItemBrowserWindow.UnloadContent();
            _mapItemBrowserWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);
            _mapItemBrowserWindow.Show();
            _mapLandBrowserWindow.UnloadContent();
            _mapLandBrowserWindow.LoadContent(GraphicsDevice, Content, _guiRenderer);
            _mapLandBrowserWindow.Show();

            _editorState = EditorState.LoggedIn;
        }

        private void NetworkClientOnLoginFailed(string reason)
        {
            _logger.Error($"Login Failed: {reason}");
        }

        private void NetworkClientConnectedHandler(EndPoint endPoint, int clientId)
        {
            _logger.Information("Connected");
            _mapConnectToServerWindow.Hide();
        }

        private void NetworkClientDisconnectedHandler()
        {
            _logger.Information("Disconnected");
        }

        private async void NetworkClientOnLoginSuccessful(Guid userId)
        {
            _logger.Information("Login Succeeded");

            var projects = await _networkClient.GetProjectsAsync();

            _mapSelectProjectsWindow.SetProjects(projects);
            _mapSelectProjectsWindow.Show();
        }

        private void DrawUi()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenuBar())
                {
                    if (_editorState == EditorState.LoggedIn)
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
                                    MapToolbarLoginClicked();
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
                                if (_showChat)
                                {
                                    _chatWindow.Show();
                                }
                                else
                                {
                                    _chatWindow.Hide();
                                }

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
                                _styleEditorWindow.ToggleVisibility();
                            }

                            if (ImGui.MenuItem("Imgui Demo"))
                            {
                                _showDemoWindow = !_showDemoWindow;
                            }

                            ImGui.EndMenu();
                        }
                    }
                    else if (_editorState == EditorState.Debug)
                    {
                        if (ImGui.BeginMenu($"DebugMode - UO Studio {_fileVersionProvider.GetVersion()}", true))
                        {
                            if (ImGui.MenuItem("Wireframe"))
                            {
                                _isWireFrame = true;
                                SetRasterizerState(_isWireFrame, _isCcw);
                            }

                            if (ImGui.MenuItem("Solid"))
                            {
                                _isWireFrame = false;
                                SetRasterizerState(_isWireFrame, _isCcw);
                            }

                            if (ImGui.MenuItem("Ccw"))
                            {
                                _isCcw = true;
                                SetRasterizerState(_isWireFrame, _isCcw);
                            }

                            if (ImGui.MenuItem("Cw"))
                            {
                                _isCcw = false;
                                SetRasterizerState(_isWireFrame, _isCcw);
                            }

                            if (ImGui.MenuItem("None"))
                            {
                                _isCcw = null;
                                SetRasterizerState(_isWireFrame, _isCcw);
                            }

                            ImGui.EndMenu();
                        }
                    }
                    else
                    {
                        if (ImGui.BeginMenu($"UO Studio {_fileVersionProvider.GetVersion()}", false))
                        {
                            ImGui.EndMenu();
                        }
                    }

                    ImGui.EndMenuBar();
                }

                ImGui.EndMainMenuBar();
            }

            if (_editorState == EditorState.Debug)
            {
                _splashScreenWindow.Hide();
                _mapConnectToServerWindow.Hide();
                _mapViewWindow.Show();
                _mapViewWindow.Draw();
            }
            else
            {
                _desktopWindow?.Draw();
                _splashScreenWindow.Draw();
                _aboutWindow?.Draw();
                _frameTimeOverlayWindow?.Draw();
                _settingsWindow?.Draw();
                _logWindow?.Draw();
                _styleEditorWindow?.Draw();
                _chatWindow?.Draw();
                if (_projectType == ProjectType.Map)
                {
                    _mapToolbarWindow?.Draw();
                    _mapViewWindow?.Draw();
                    _mapItemBrowserWindow?.Draw();
                    _mapLandBrowserWindow?.Draw();
                    _mapTileDetailWindow?.Draw();
                    _mapTilePreviewWindow?.Draw();
                    _mapConnectToServerWindow?.Draw();
                    _mapViewProfileWindow?.Draw();
                    _mapSelectProjectsWindow?.Draw();
                    _mapCreateProjectWindow?.Draw();
                    _mapAddOrEditProfileWindow?.Draw();
                }
            }

            if (_showDemoWindow)
            {
                ImGui.ShowDemoWindow();
            }

            if (_networkClientDownloadPercentage > 0.0 && _networkClientDownloadPercentage < 100.0)
            {
                var viewPort = ImGui.GetWindowViewport();
                ImGui.SetNextWindowPos(new Vector2(viewPort.Size.X / 2.0f - 200, viewPort.Size.Y / 2.0f - 18));
                ImGui.SetNextWindowSize(new Vector2(400, 36));
                if (ImGui.Begin("Loading Assets", ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize))
                {
                    ImGui.ProgressBar((float)_networkClientDownloadPercentage / 100.0f, new Vector2(-1, 0), $"{_networkClientDownloadPercentage:F2}");
                    ImGui.End();
                }
            }
        }

        private bool _isWireFrame;
        private bool? _isCcw;

        private void SetRasterizerState(bool wireFrame, bool? ccw)
        {
            _wireframeRasterizerState?.Dispose();
            _wireframeRasterizerState = new RasterizerState();
            _wireframeRasterizerState.CullMode = ccw == null
                ? CullMode.None
                : ccw.Value
                    ? CullMode.CullCounterClockwiseFace
                    : CullMode.CullClockwiseFace;
            _wireframeRasterizerState.FillMode = wireFrame ? FillMode.WireFrame : FillMode.Solid;
        }
    }
}
