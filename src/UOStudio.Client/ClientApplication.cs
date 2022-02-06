using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGuiNET;
using LiteNetLib;
using Serilog;
using UOStudio.Client.Engine;
using UOStudio.Client.Engine.Graphics;
using UOStudio.Client.Engine.Input;
using UOStudio.Client.Engine.Mathematics;
using UOStudio.Client.Engine.Native.OpenGL;
using UOStudio.Client.Engine.UI;
using UOStudio.Client.Services;
using UOStudio.Client.UI;
using UOStudio.Client.Worlds;
using UOStudio.Common.Contracts;
using UOStudio.Common.Network;
using UOStudio.Tools.TextureAtlasGenerator.Contracts;
using Num = System.Numerics;

namespace UOStudio.Client
{
    internal sealed class ClientApplication : Application
    {
        private readonly Vector3 _clearColor = new Vector3(0.1f, 0.1f, 0.1f);
        private readonly ILogger _logger;
        private readonly IGraphicsDevice _graphicsDevice;
        private ImGuiController _imGuiController;

        private readonly ClientStartParameters _clientStartParameters;
        private readonly IMeshLibrary _meshLibrary;
        private readonly IMaterialLibrary _materialLibrary;
        private readonly IShaderProgramLibrary _shaderProgramLibrary;

        private readonly INetworkClient _networkClient;
        private readonly IWindowProvider _windowProvider;
        private readonly IWorldProvider _worldProvider;
        private readonly IWorldChunkProvider _worldChunkProvider;
        private readonly IContext _context;
        private readonly IWorldRenderer _worldRenderer;
        private readonly ITextureAtlasProvider _textureAtlasProvider;

        private IInputLayout _worldChunkInputLayout;
        private IBuffer _worldChunkVertexBuffer;
        private IBuffer _worldChunkIndexBuffer;
        private IShader _worldChunkShader;

        private ITextureAtlas _textureAtlas;
        private int _textureAtlasViewIndex;
        private int _landTileIndex;
        private int _landTextureIndex;
        private int _itemTileIndex;
        private ItemTile _selectedItemTile;
        private LandTile _selectedLandTile;
        private LandTile _selectedLandTexture;

        private readonly Camera _camera;

        private World _currentWorld;

        public ClientApplication(
            ILogger logger,
            WindowSettings windowSettings,
            ContextSettings contextSettings,
            IWindowFactory windowFactory,
            IGraphicsDevice graphicsDevice,
            ClientStartParameters clientStartParameters,
            IMeshLibrary meshLibrary,
            IMaterialLibrary materialLibrary,
            IShaderProgramLibrary shaderProgramLibrary,
            INetworkClient networkClient,
            IWindowProvider windowProvider,
            IWorldProvider worldProvider,
            IWorldChunkProvider worldChunkProvider,
            IWorldRenderer worldRenderer,
            ITextureAtlasProvider textureAtlasProvider,
            IContext context)
            : base(logger, windowSettings, contextSettings, windowFactory, graphicsDevice)
        {
            _logger = logger;
            _graphicsDevice = graphicsDevice;

            _clientStartParameters = clientStartParameters;
            _meshLibrary = meshLibrary;
            _materialLibrary = materialLibrary;
            _shaderProgramLibrary = shaderProgramLibrary;

            _networkClient = networkClient;
            _networkClient.Connected += NetworkClientConnected;
            _networkClient.Disconnected += NetworkClientDisconnected;
            _networkClient.ChunkReceived += NetworkClientChunkReceived;

            _windowProvider = windowProvider;
            _worldChunkProvider = worldChunkProvider;
            _context = context;
            _windowProvider.Load();

            _worldProvider = worldProvider;
            _worldRenderer = worldRenderer;
            _textureAtlasProvider = textureAtlasProvider;

            _camera = new Camera(FrameWidth, FrameHeight, new Vector3(0, 0, 3), Vector3.Up);
        }

        protected override bool Load()
        {
            var sw = Stopwatch.StartNew();
            _logger.Information("App: Initializing...");
            if (!LoadShaders())
            {
                return false;
            }

            //if (!_textureAtlasProvider.TryLoadTextureAtlas("Atlas_7_0_50_0", TextureFormat.Rgba8, out _textureAtlas))
            if (!_textureAtlasProvider.TryLoadTextureAtlas("External", TextureFormat.Rgba8, out _textureAtlas))
            {
                return false;
            }

            _imGuiController = new ImGuiController(_graphicsDevice, ResolutionWidth, ResolutionHeight);

            _currentWorld = _worldProvider.GetWorld(_clientStartParameters.ProjectId);

            sw.Stop();
            _logger.Information("App: Initializing...Done. Took {@TotalSeconds}s", sw.Elapsed.TotalSeconds);

            _logger.Information("Content: Loading...");
            _worldRenderer.Load(_graphicsDevice);
            _logger.Information("Content: Loading...Done");

            _networkClient.Connect("localhost", 9050);

            GL.Disable(GL.EnableCap.Multisample);
            GL.Enable(GL.EnableCap.ScissorTest);
            GL.Enable(GL.EnableCap.DepthTest);
            GL.Enable(GL.EnableCap.CullFace);
            GL.Enable(GL.EnableCap.Blend);
            GL.CullFace(GL.CullFaceMode.Front);
            GL.FrontFace(GL.FrontFaceDirection.Cw);
            GL.DepthFunc(GL.DepthFunction.Less);
            GL.BlendEquation(GL.BlendEquationMode.FuncAdd);
            GL.BlendFunc(GL.BlendingFactor.One, GL.BlendingFactor.OneMinusSrcAlpha);

            var world = _worldProvider.GetWorld(0);
            var worldChunk = world.GetChunk(new Point(0, 0));

            var staticTileData = new ChunkStaticTileData[ChunkData.ChunkSize * ChunkData.ChunkSize];
            var rnd = new Random();
            for (var tileIndex = 0; tileIndex < staticTileData.Length; tileIndex++)
            {
                staticTileData[tileIndex] = new ChunkStaticTileData((ushort)rnd.Next(0, _textureAtlas.LandTextureCount), 0, 0);
            }
            staticTileData[7] = new ChunkStaticTileData(26, 1, 0);
            staticTileData[3] = new ChunkStaticTileData(1529, 3, 0);
            staticTileData[5] = new ChunkStaticTileData(457, 2, 0);
            staticTileData[15] = new ChunkStaticTileData(27, 1, 0);

            var itemTileData = new ChunkItemTileData[ChunkData.ChunkSize * ChunkData.ChunkSize];

            var chunkData = new ChunkData(0, Point.Zero, staticTileData, null);
            worldChunk.UpdateWorldChunk(ref chunkData);

            _worldChunkVertexBuffer = BuildVertexBuffer(worldChunk);
            _worldChunkInputLayout = _graphicsDevice.GetInputLayout(VertexType.PositionColorNormalUvw);
            _worldChunkInputLayout.AddVertexBuffer(_worldChunkVertexBuffer, 0);

            return true;
        }

        protected override void Render(float elapsedTime, float deltaTime)
        {
            if (!IsFocused)
            {
                return;
            }

            GL.Enable(GL.EnableCap.DepthTest);
            _graphicsDevice.Clear(_clearColor);

            _worldChunkInputLayout.Bind();
            _worldChunkShader.SetVertexUniform("u_projection", _camera.ProjectionMatrix);
            _worldChunkShader.SetVertexUniform("u_view", _camera.ViewMatrix);
            _worldChunkShader.SetVertexUniform("u_world", Matrix.Identity);
            _worldChunkShader.Bind();
            _textureAtlas.AtlasTexture.Bind(0);
            GL.DrawArrays(GL.PrimitiveType.Triangles, 0, _worldChunkVertexBuffer.Count);

//            _worldRenderer.Draw(_graphicsDevice, _currentWorld, _camera);

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
            _logger.Information("Network: Disconnecting...");
            _networkClient.Disconnect();

            _logger.Information("Content: Unloading...");
            _imGuiController?.Dispose();
            _logger.Information("Content: Unloading...Done");
            base.Unload();
        }

        protected override void Update(float elapsedTime, float deltaTime)
        {
            _networkClient.Update();

            if (!IsFocused)
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

        private IBuffer BuildVertexBuffer(WorldChunk worldChunk)
        {
            var landVertices = new List<VertexPositionColorNormalUvw>();

            const int TileSize = 44;
            const int TileSizeHalf = TileSize / 2;

            void DrawSquare(int landId, int x, int y, float tileZ, float tileZEast, float tileZWest, float tileZSouth)
            {
                var tile = tileZ != 0
                    ? _textureAtlas.GetLandTile(landId)
                    : _textureAtlas.GetLandTextureTile(landId);

                // tile = _textureAtlas.GetLandTile(landId);

                var p0 = new Vector3(x + TileSizeHalf, y - tileZ * 4, 0);
                var p1 = new Vector3(x + TileSize, y + TileSizeHalf - tileZEast * 4, 0);
                var p2 = new Vector3(x, y + TileSizeHalf - tileZWest * 4, 0);
                var p3 = new Vector3(x + TileSizeHalf, y + TileSize - tileZSouth * 4, 0);

                var uv0 = new Vector3(tile.Uvws.V1.U, tile.Uvws.V1.V, tile.Uvws.V1.W);
                var uv1 = new Vector3(tile.Uvws.V2.U, tile.Uvws.V2.V, tile.Uvws.V2.W);
                var uv2 = new Vector3(tile.Uvws.V3.U, tile.Uvws.V3.V, tile.Uvws.V3.W);
                var uv3 = new Vector3(tile.Uvws.V4.U, tile.Uvws.V4.V, tile.Uvws.V4.W);

                var n1 = Vector3.Cross(p2 - p0, p1 - p0);
                landVertices.Add(new VertexPositionColorNormalUvw(p0, Color.White.ToVector3(), n1, uv0));
                landVertices.Add(new VertexPositionColorNormalUvw(p1, Color.White.ToVector3(), n1, uv1));
                landVertices.Add(new VertexPositionColorNormalUvw(p2, Color.White.ToVector3(), n1, uv2));

                var n2 = Vector3.Cross(p2 - p3, p1 - p3);
                landVertices.Add(new VertexPositionColorNormalUvw(p1, Color.White.ToVector3(), n2, uv1));
                landVertices.Add(new VertexPositionColorNormalUvw(p3, Color.White.ToVector3(), n2, uv3));
                landVertices.Add(new VertexPositionColorNormalUvw(p2, Color.White.ToVector3(), n2, uv2));
            }

            for (var y = 0; y < ChunkData.ChunkSize; ++y)
            {
                for (var x = 0; x < ChunkData.ChunkSize; ++x)
                {
                    var staticTile = worldChunk.GetStaticTile(x, y);

                    var tileZ = worldChunk.GetZ(x, y);
                    var tileZEast = worldChunk.GetZ(x + 1, y);
                    var tileZWest = worldChunk.GetZ(x, y + 1);
                    var tileZSouth = worldChunk.GetZ(x + 1, y + 1);

                    DrawSquare(staticTile.TileId, (x - y) * TileSizeHalf, (x + y) * TileSizeHalf, tileZ, tileZEast, tileZWest, tileZSouth);
                }
            }

            return _graphicsDevice.CreateBuffer("VB_Land", landVertices);
        }

        private bool LoadShaders()
        {
            if (!_shaderProgramLibrary.AddShaderProgram("Chunk", "Shaders/Chunk.vs.glsl", "Shaders/Chunk.fs.glsl"))
            {
                return false;
            }

            _worldChunkShader = _shaderProgramLibrary.GetShaderProgram("Chunk");

            return true;
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

            if (ImGui.Begin("Textures"))
            {
                ImGui.TextUnformatted($"Items: {_itemTileIndex} / {_textureAtlas.ItemTileCount}");
                ImGui.TextUnformatted($"Land Tiles: {_landTileIndex} / {_textureAtlas.LandTileCount}");
                ImGui.TextUnformatted($"Land Textures: {_landTextureIndex} / {_textureAtlas.LandTextureCount}");

                var itemTileIndex = _itemTileIndex;
                if (ImGui.SliderInt("Selected Item Tile", ref itemTileIndex, 0, _textureAtlas.ItemTileCount))
                {
                    _itemTileIndex = itemTileIndex;
                }
                var landTileIndex = _landTileIndex;
                if (ImGui.SliderInt("Selected Land Tile", ref landTileIndex, 0, _textureAtlas.LandTileCount))
                {
                    _landTileIndex = landTileIndex;
                }
                var landTextureIndex = _landTextureIndex;

                ImGui.TextUnformatted("Land Texture");
                ImGui.SameLine();
                if (ImGui.Button("-100"))
                {
                    _landTextureIndex -= 100;
                    if (_landTextureIndex < 0)
                    {
                        _landTextureIndex = 0;
                    }
                }
                ImGui.SameLine();
                if (ImGui.Button("-10"))
                {
                    _landTextureIndex -= 10;
                    if (_landTextureIndex < 0)
                    {
                        _landTextureIndex = 0;
                    }
                }
                ImGui.SameLine();
                if (ImGui.Button("-1"))
                {
                    _landTextureIndex -= 1;
                    if (_landTextureIndex < 0)
                    {
                        _landTextureIndex = 0;
                    }
                }
                ImGui.SameLine();
                ImGui.SetNextItemWidth(200.0f);
                if (ImGui.SliderInt("##Selected Land Texture", ref landTextureIndex, 0, _textureAtlas.LandTextureCount))
                {
                    _landTextureIndex = landTextureIndex;
                }
                ImGui.SameLine();
                if (ImGui.Button("+1"))
                {
                    _landTextureIndex += 1;
                    if (_landTextureIndex >= _textureAtlas.LandTextureCount)
                    {
                        _landTextureIndex = _textureAtlas.LandTextureCount;
                    }
                }
                ImGui.SameLine();
                if (ImGui.Button("+10"))
                {
                    _landTextureIndex += 10;
                    if (_landTextureIndex >= _textureAtlas.LandTextureCount)
                    {
                        _landTextureIndex = _textureAtlas.LandTextureCount;
                    }
                }
                ImGui.SameLine();
                if (ImGui.Button("+100"))
                {
                    _landTextureIndex += 100;
                    if (_landTextureIndex >= _textureAtlas.LandTextureCount)
                    {
                        _landTextureIndex = _textureAtlas.LandTextureCount;
                    }
                }

                _selectedItemTile = _textureAtlas.GetItemTile(itemTileIndex);
                _selectedLandTile = _textureAtlas.GetLandTile(landTileIndex);
                _selectedLandTexture = _textureAtlas.GetLandTextureTile(landTextureIndex);

                var itemTileTextureView = _textureAtlas.AtlasTextureViews[(int)_selectedItemTile.Uvws.V1.W];
                ImGui.Image(
                    itemTileTextureView.AsIntPtr(),
                    new Num.Vector2(_selectedItemTile.Width, _selectedItemTile.Height),
                    new Num.Vector2(_selectedItemTile.Uvws.V1.U, _selectedItemTile.Uvws.V1.V),
                    new Num.Vector2(_selectedItemTile.Uvws.V4.U, _selectedItemTile.Uvws.V4.V));

                var landTileTextureView = _textureAtlas.AtlasTextureViews[(int)_selectedLandTile.Uvws.V1.W];
                ImGui.Image(
                    landTileTextureView.AsIntPtr(),
                    new Num.Vector2(_selectedLandTile.Width, _selectedLandTile.Height),
                    new Num.Vector2(_selectedLandTile.Uvws.V1.U, _selectedLandTile.Uvws.V4.U),
                    new Num.Vector2(_selectedLandTile.Uvws.V1.V, _selectedLandTile.Uvws.V4.V), Num.Vector4.One, Num.Vector4.One);

                var landTextureTextureView = _textureAtlas.AtlasTextureViews[(int)_selectedLandTexture.Uvws.V1.W];
                ImGui.Image(
                    landTextureTextureView.AsIntPtr(),
                    new Num.Vector2(_selectedLandTexture.Width, _selectedLandTexture.Height),
                    new Num.Vector2(_selectedLandTexture.Uvws.V1.U, _selectedLandTexture.Uvws.V1.V),
                    new Num.Vector2(_selectedLandTexture.Uvws.V4.U, _selectedLandTexture.Uvws.V4.V));

                ImGui.End();
            }

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
            var chunk = _worldChunkProvider.GetChunk(_clientStartParameters.ProjectId, chunkData.Position);
            chunk.UpdateWorldChunk(ref chunkData);
            _worldRenderer.UpdateChunk(chunk);
        }
    }
}
