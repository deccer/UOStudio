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
using Newtonsoft.Json;
using Serilog;
using UOStudio.Client.Core;
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

    public struct MapTile
    {
        [JsonProperty("Id")]
        public int TileId { get; set; }

        public float U0 { get; set; }

        public float V0 { get; set; }

        public float U1 { get; set; }

        public float V1 { get; set; }

        public float U2 { get; set; }

        public float V2 { get; set; }

        public float U3 { get; set; }

        public float V3 { get; set; }
    }

    public class ClientGame : Game
    {
        private readonly ILogger _logger;
        private readonly IFileVersionProvider _fileVersionProvider;
        private readonly IProfileService _profileService;
        private readonly INetworkClient _networkClient;
        private readonly GraphicsDeviceManager _graphics;
        private ImGuiRenderer _guiRenderer;
        private readonly Camera _camera;
        private bool _isWindowFocused;

        private KeyboardState _currentKeyboardState;
        private MouseState _currentMouseState;

        private readonly Color _clearColor = new Color(0.1f, 0.1f, 0.1f);

        private bool _showChat = true;
        private bool _showDemoWindow = false;

        private IList<MapTile> _mapTilesInfo;
        private Texture2D _mapTilesAtlas;
        private SamplerState _mapAtlasSamplerState;

        private ItemProvider _itemProvider;
        private TileDataProvider _tileDataProvider;

        private ProjectType _projectType;

        private RenderTarget2D _mapViewRenderTarget;
        private IntPtr _mapViewRenderTargetTextureId;
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

        private BasicEffect _mapEffect;
        private VertexBuffer _mapVertexBuffer;
        private IList<VertexPositionColorTexture> _mapVertices;
        private RasterizerState _wireframeRasterizerState;
        private IDictionary<(int, int), Tile> _mapTiles;

        private const int NumberSamples = 50;
        private float _fps;
        private readonly int[] _samples = new int[NumberSamples];
        private int _currentSample;
        private int ticksAggregate;

        public ClientGame(
            ILogger logger,
            IFileVersionProvider fileVersionProvider,
            IProfileService profileService,
            INetworkClient networkClient
        )
        {
            _logger = logger;
            _fileVersionProvider = fileVersionProvider;
            _profileService = profileService;
            _networkClient = networkClient;

            FNALoggerEXT.LogError = message => _logger.Error($"FNA: {message}");
            FNALoggerEXT.LogInfo = message => _logger.Information($"FNA: {message}");
            FNALoggerEXT.LogWarn = message => _logger.Warning($"FNA: {message}");

            _networkClient.Connected += NetworkClientConnectedHandler;
            _networkClient.Disconnected += NetworkClientDisconnectedHandler;
            _networkClient.LoginSuccessful += NetworkClientOnLoginSuccessful;
            _networkClient.LoginFailed += NetworkClientOnLoginFailed;
            _networkClient.JoinProjectSuccessful += NetworkClientOnJoinProjectSuccessful;
            _networkClient.JoinProjectFailed += NetworkClientOnJoinProjectFailed;
            _networkClient.DownloadProgress += NetworkClientOnDownloadProgress;

            Window.Title = "UOStudio";
            Window.AllowUserResizing = true;
            Activated += (_, _) =>
            {
                _isWindowFocused = true;
                IsFixedTimeStep = false;
            };
            Deactivated += (_, _) =>
            {
                _isWindowFocused = false;
                IsFixedTimeStep = true;
            };

            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
                PreferMultiSampling = true,
                GraphicsProfile = GraphicsProfile.HiDef,
                SynchronizeWithVerticalRetrace = false
            };
            _graphics.ApplyChanges();
            _camera = new Camera(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            _camera.Mode = CameraMode.Perspective;

            _projectType = ProjectType.Map;

            IsMouseVisible = true;
            IsFixedTimeStep = false;
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
            _settingsWindow = new SettingsWindow();
            _logWindow = new LogWindow();
            _styleEditorWindow = new StyleEditorWindow();
            _chatWindow = new ChatWindow();

            _mapVertices = new List<VertexPositionColorTexture>();

            base.Initialize();
            _logger.Information("Initializing...Done");
            _logWindow.AddLogMessage(LogType.Info, "Initializing...Done");

            int DesiredFrameRate = 60;
            TargetElapsedTime = new TimeSpan(TimeSpan.TicksPerSecond / DesiredFrameRate);
        }

        private float Sum(int[] samples)
        {
            float RetVal = 0f;
            for (int i = 0; i < samples.Length; i++)
            {
                RetVal += samples[i];
            }
            return RetVal;
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!_isWindowFocused)
            {
                return;
            }
            _samples[_currentSample++] = (int)gameTime.ElapsedGameTime.Ticks;
            ticksAggregate += (int)gameTime.ElapsedGameTime.Ticks;
            if (ticksAggregate > TimeSpan.TicksPerSecond)
            {
                ticksAggregate -= (int)TimeSpan.TicksPerSecond;
            }
            if (_currentSample == NumberSamples)
            {
                float AverageFrameTime = Sum(_samples) / NumberSamples;
                _fps = TimeSpan.TicksPerSecond / AverageFrameTime;
                _currentSample = 0;
            }

            GraphicsDevice.SetRenderTarget(_mapViewRenderTarget);
            GraphicsDevice.Viewport = new Viewport(_mapViewRenderTarget.Bounds);
            GraphicsDevice.Clear(Color.SlateGray);

            _mapEffect.World = Matrix.Identity * Matrix.CreateScale(new Vector3(1, -1, 1));
            _mapEffect.View = _camera.ViewMatrix;
            _mapEffect.Projection = _camera.ProjectionMatrix;
            _mapEffect.VertexColorEnabled = false;
            _mapEffect.Texture = _mapTilesAtlas;
            _mapEffect.TextureEnabled = true;

            GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;

            GraphicsDevice.SetVertexBuffer(_mapVertexBuffer);
            GraphicsDevice.RasterizerState = _wireframeRasterizerState;
            foreach (var pass in _mapEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, _mapVertices.Count / 3);
            }

            GraphicsDevice.SetVertexBuffer(null);
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Viewport = new Viewport(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            GraphicsDevice.Clear(_clearColor);

            _guiRenderer.BeginLayout(gameTime);
            DrawUi();
            _guiRenderer.EndLayout();

            Window.Title = $"FPS: {_fps}";
            base.Draw(gameTime);
        }

        protected override void LoadContent()
        {
            var map = new Engine.Ultima.Map(@"D:\Private\Code\Projects\UOStudio\src\UOStudio.Client\bin\Debug\Projects\Temp", "Felucca");
            var tm = map.Tiles;
            var w = 64;
            var h = 64;
            var startx = 220;
            var starty = 325;
            _mapTiles = new Dictionary<(int, int), Tile>(131072);

            for (var bx = startx; bx < startx + w; bx++)
            {
                for (var by = starty; by < starty + h; by++)
                {
                    var lb = tm.GetLandBlock(bx, by);

                    var bbx = bx - startx;
                    var bby = by - starty;

                    for (var cx = 0; cx < 8; cx++)
                    {
                        for (var cy = 0; cy < 8; cy++)
                        {
                            var lt = lb[cy * 8 + cx];
                            var x = bbx * 8 + cx;
                            var y = bby * 8 + cy;
                            _mapTiles.Add((x, y), new Tile(lt.ID, lt.Z));
                        }
                    }
                }
            }

            _logger.Information("Content - Loading...");
            base.LoadContent();

            _mapViewRenderTarget = new RenderTarget2D(
                GraphicsDevice,
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8
            );

            _mapTilesAtlas = Content.Load<Texture2D>("Content/Atlas.png");
            _mapTilesInfo = new List<MapTile>(16384);
            var json = File.ReadAllText($"Content/Atlas.json");
            _mapTilesInfo = JsonConvert.DeserializeObject<IList<MapTile>>(json);

            _mapVertexBuffer?.Dispose();
            _mapVertexBuffer = BuildVertexBuffer(w * 8, h * 8, _mapTilesInfo);
            _mapEffect = new BasicEffect(GraphicsDevice);
            _mapAtlasSamplerState = SamplerState.PointClamp;

            _wireframeRasterizerState = new RasterizerState
            {
                CullMode = CullMode.CullCounterClockwiseFace,
                FillMode = FillMode.WireFrame,
                MultiSampleAntiAlias = true
            };

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

            _mapViewRenderTargetTextureId = _guiRenderer.BindTexture(_mapViewRenderTarget);
            _mapViewWindow = new MapViewWindow(_editorState);
            _mapViewWindow.OnWindowResize += MapViewWindowOnOnWindowResize;
            _mapViewWindow.Show();
            _mapViewWindow.UpdateRenderTarget(_mapViewRenderTargetTextureId, _mapViewRenderTarget.Width, _mapViewRenderTarget.Height);

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

        private void MapViewWindowOnOnWindowResize(Vector2 viewSize)
        {
            _guiRenderer.UnbindTexture(_mapViewRenderTargetTextureId);
            _mapViewRenderTarget?.Dispose();
            _mapViewRenderTarget = new RenderTarget2D(GraphicsDevice, (int)viewSize.X, (int)viewSize.Y, false,
                SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            _mapViewRenderTargetTextureId = _guiRenderer.BindTexture(_mapViewRenderTarget);
            _mapViewWindow.UpdateRenderTarget(_mapViewRenderTargetTextureId, _mapViewRenderTarget.Width, _mapViewRenderTarget.Height);
        }

        private float GetTileZ(int x, int y, float defaultZ) => _mapTiles.TryGetValue((x, y), out var tile)
            ? tile.Z
            : defaultZ;

        private VertexBuffer BuildVertexBuffer(int width, int height, IList<MapTile> mapTilesInfo)
        {
            const int TileSize = 44;
            const int TileSizeHalf = TileSize / 2;

            void DrawSquare(int graphicId, int x, int y, float tileZ, float tileZEast, float tileZWest, float tileZSouth)
            {
                var p0 = new Vector3(x + TileSizeHalf, y - tileZ * 4, 0);
                var p1 = new Vector3(x + TileSize, y + TileSizeHalf - tileZEast * 4, 0);
                var p2 = new Vector3(x, y + TileSizeHalf - tileZWest * 4, 0);
                var p3 = new Vector3(x + TileSizeHalf, y + TileSize - tileZSouth * 4, 0);

                var mapTile = mapTilesInfo[graphicId];
                var uv0 = new Microsoft.Xna.Framework.Vector2(mapTile.U0, mapTile.V0);
                var uv1 = new Microsoft.Xna.Framework.Vector2(mapTile.U1, mapTile.V1);
                var uv2 = new Microsoft.Xna.Framework.Vector2(mapTile.U2, mapTile.V2);
                var uv3 = new Microsoft.Xna.Framework.Vector2(mapTile.U3, mapTile.V3);

                var n1 = Vector3.Cross(p2 - p0, p1 - p0);
                _mapVertices.Add(new VertexPositionColorTexture(p0, Color.White, uv0));
                _mapVertices.Add(new VertexPositionColorTexture(p1, Color.White, uv1));
                _mapVertices.Add(new VertexPositionColorTexture(p2, Color.White, uv2));

                var n2 = Vector3.Cross(p2 - p3, p1 - p3);
                _mapVertices.Add(new VertexPositionColorTexture(p1, Color.Black, uv1));
                _mapVertices.Add(new VertexPositionColorTexture(p3, Color.Black, uv3));
                _mapVertices.Add(new VertexPositionColorTexture(p2, Color.Black, uv2));
            }

            for (var x = 0; x < width; ++x)
            {
                for (var y = 0; y < height; ++y)
                {
                    var tileZ = GetTileZ(x, y, -15);
                    var tileZEast = GetTileZ(x + 1, y, tileZ);
                    var tileZWest = GetTileZ(x, y + 1, tileZ);
                    var tileZSouth = GetTileZ(x + 1, y + 1, tileZ);
                    var graphicId = _mapTiles[(x, y)].TileId;

                    DrawSquare(graphicId, (x - y) * TileSizeHalf, (x + y) * TileSizeHalf - 200, tileZ, tileZEast, tileZWest, tileZSouth);
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
                _profileService.RemoveProfile(profile);
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

            var encoded = Encoding.ASCII.GetBytes($"{profile.UserName}:{profile.UserPassword}");
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
            if (!_isWindowFocused)
            {
                return;
            }
            _networkClient.Update();
            var previousKeyboardState = _currentKeyboardState;
            var previousMouseState = _currentMouseState;
            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();

            _camera.Update(_mapViewRenderTarget.Width, _mapViewRenderTarget.Height);

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
