using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private readonly IContext _context;
        private readonly IWorldRenderer _worldRenderer;

        private IShader _basicShader;
        private ITextureArray _basicTextureAtlas;
        private IInputLayout _basicInputLayout;
        private IBuffer _basicVertexBuffer;
        private IBuffer _basicIndexBuffer;
        private int _uvIndex;

        private readonly Camera _camera;

        private World _currentWorld;

        public ClientApplication(
            ILogger logger,
            WindowSettings windowSettings,
            ContextSettings contextSettings,
            IGraphicsDevice graphicsDevice,
            ClientStartParameters clientStartParameters,
            IMeshLibrary meshLibrary,
            IMaterialLibrary materialLibrary,
            IShaderProgramLibrary shaderProgramLibrary,
            INetworkClient networkClient,
            IWindowProvider windowProvider,
            IWorldProvider worldProvider,
            IWorldRenderer worldRenderer,
            IContext context)
            : base(logger, windowSettings, contextSettings, graphicsDevice)
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
            _context = context;
            _windowProvider.Load();

            _worldProvider = worldProvider;
            _worldRenderer = worldRenderer;

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

            var meshDates = MeshData.CreateMeshDataFromFile("Models/SM_UnitCube.fbx", _materialLibrary);
            var meshData = MeshData.Combine(meshDates.ToArray());
            _basicVertexBuffer = meshData.CreateVertexBuffer(_graphicsDevice);
            _basicIndexBuffer = meshData.CreateIndexBuffer(_graphicsDevice);
            _basicInputLayout = _graphicsDevice.GetInputLayout(meshData.VertexType);
            _basicInputLayout.AddVertexBuffer(_basicVertexBuffer, 0);
            _basicInputLayout.AddElementBuffer(_basicIndexBuffer);

            var imageBytes = File.ReadAllBytes("C:\\Temp\\TestBytes.bytes");
            _basicTextureAtlas = _graphicsDevice.CreateTextureArrayFromBytes(128, 128, 5, imageBytes, TextureFormat.Rgba8);

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

            _basicInputLayout.Bind();
            _basicShader.SetVertexUniform("u_projection", _camera.ProjectionMatrix);
            _basicShader.SetVertexUniform("u_view", _camera.ViewMatrix);
            _basicShader.SetVertexUniform("u_world", Matrix.Identity);
            _basicShader.SetFragmentUniform("u_uv_index", _uvIndex);
            _basicShader.Bind();
            _basicTextureAtlas.Bind(0);

            GL.DrawElementsInstanced(GL.PrimitiveType.Triangles, _basicIndexBuffer.Count, GL.DrawElementsType.UnsignedInt, 0, 1);

            _worldRenderer.Draw(_graphicsDevice, _currentWorld, _camera);

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

        private bool LoadShaders()
        {
            if (!_shaderProgramLibrary.AddShaderProgram("Basic", "Shaders/Basic.vs.glsl", "Shaders/Basic.fs.glsl"))
            {
                return false;
            }

            _basicShader = _shaderProgramLibrary.GetShaderProgram("Basic");

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

            if (ImGui.Begin("Debug"))
            {
                var uvIndex = _uvIndex;
                if (ImGui.SliderInt("Uv Index", ref uvIndex, 0, 4))
                {
                    _uvIndex = uvIndex;
                }
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
            var chunk = _worldProvider.GetChunk(_clientStartParameters.ProjectId, chunkData.Position);
            chunk.UpdateWorldChunk(ref chunkData);
            _worldRenderer.UpdateChunk(chunk);
        }
    }
}
