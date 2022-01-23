using System.Diagnostics;
using System.Runtime.InteropServices;
using CSharpFunctionalExtensions;
using Serilog;
using SixLabors.ImageSharp;
using UOStudio.Client.Engine.Mathematics;
using UOStudio.Client.Engine.Native;
using UOStudio.Client.Engine.Native.OpenGL;
using Color = UOStudio.Client.Engine.Mathematics.Color;

namespace UOStudio.Client.Engine.Graphics
{
    internal sealed class GraphicsDevice : IGraphicsDevice
    {
        private readonly ILogger _logger;
        private readonly IInputLayoutProvider _inputLayoutProvider;
        private readonly GL.GLDebugProc _debugProcCallback;
        private IntPtr _renderContext;
        private Vector3 _clearColor;
        private bool _vSync;

        public bool VSync
        {
            get { return _vSync; }
            set
            {
                if (_vSync != value)
                {
                    _vSync = value;
                    Sdl.SetSwapInterval(_vSync ? 1 : 0);
                }
            }
        }

        public GraphicsDevice(
            ILogger logger,
            IInputLayoutProvider inputLayoutProvider)
        {
            _logger = logger;
            _inputLayoutProvider = inputLayoutProvider;
            _debugProcCallback = DebugCallback;
        }

        public void Dispose()
        {
            _inputLayoutProvider.Dispose();
            if (_renderContext != IntPtr.Zero)
            {
                Sdl.DeleteRenderContext(_renderContext);
            }
        }

        public bool Initialize(ContextSettings contextSettings, IntPtr windowHandle)
        {
            var targetOpenGLVersion = Version.Parse(contextSettings.TargetGLVersion);
            if (contextSettings.IsDebugContext)
            {
                Sdl.SetGlAttribute(Sdl.GlAttribute.ContextFlags, Sdl.SdlContext.Debug);
            }
            Sdl.SetGlAttribute(Sdl.GlAttribute.Profile, Sdl.SdlProfile.Core);
            Sdl.SetGlAttribute(Sdl.GlAttribute.ContextMajorVersion, targetOpenGLVersion.Major);
            Sdl.SetGlAttribute(Sdl.GlAttribute.ContextMinorVersion, targetOpenGLVersion.Minor);
            _renderContext = Sdl.CreateRenderContext(windowHandle);
            if (_renderContext == IntPtr.Zero)
            {
                _logger.Error("SDL: GL_CreateContext failed");
                return false;
            }

            Sdl.MakeCurrent(windowHandle, _renderContext);

            if (contextSettings.IsDebugContext)
            {
                GL.DebugMessageCallback(_debugProcCallback, IntPtr.Zero);
                GL.Enable(GL.EnableCap.DebugOutput);
                GL.Enable(GL.EnableCap.DebugOutputSynchronous);
            }

            return true;
        }

        public void Clear(Vector3 clearColor)
        {
            if (!_clearColor.Equals(clearColor))
            {
                GL.ClearColor(clearColor.X, clearColor.Y, clearColor.Z, 1.0f);
                _clearColor = clearColor;
            }
            GL.Clear(GL.ClearBufferMask.ColorBufferBit | GL.ClearBufferMask.DepthBufferBit);
        }

        public Result<IShader> CreateShaderProgramFromFiles(
            string label,
            string vertexShaderFileName,
            string fragmentShaderFileName)
        {
            return Shader.FromFiles(label, vertexShaderFileName, fragmentShaderFileName);
        }

        public Result<IShader> CreateShaderProgramFromSources(
            string label,
            string vertexShaderSource,
            string fragmentShaderSource)
        {
            return Shader.FromSources(label, vertexShaderSource, fragmentShaderSource);
        }

        public IFramebuffer CreateFramebuffer(
            string label,
            ITexture[] colorAttachments,
            ITexture depthAttachment = null)
        {
            return new Framebuffer(label, colorAttachments, depthAttachment);
        }

        public ITexture CreateDepthTexture(
            int width,
            int height,
            TextureFormat textureFormat)
        {
            return new Texture(width, height, textureFormat, MinFilter.Nearest, MagFilter.Nearest, WrapMode.ClampToBorder,
                IntPtr.Zero, "DepthBuffer");
        }

        public ITexture CreateTexture(
            string label,
            int width,
            int height,
            TextureFormat textureFormat,
            MagFilter magFilter = MagFilter.Nearest,
            MinFilter minFilter = MinFilter.Nearest,
            WrapMode wrapMode = WrapMode.Repeat,
            IntPtr? initialData = null)
        {
            return initialData == null
                ? new Texture(width, height, textureFormat, minFilter, magFilter, wrapMode, name: label)
                : new Texture(width, height, textureFormat, minFilter, magFilter, wrapMode, initialData.Value, label);
        }

        public ITexture CreateTextureFromFile(
            string fileName,
            MagFilter magFilter = MagFilter.Linear,
            MinFilter minFilter = MinFilter.Linear,
            WrapMode wrapMode = WrapMode.Repeat)
        {
            return new Texture(fileName, minFilter, magFilter, wrapMode);
        }

        public ITextureArray CreateTextureArrayFromImages(
            IEnumerable<Image> images,
            TextureFormat textureFormat,
            MagFilter magFilter = MagFilter.Nearest,
            MinFilter minFilter = MinFilter.Nearest,
            WrapMode wrapMode = WrapMode.Repeat)
        {
            return new TextureArray(images, textureFormat, magFilter, minFilter, wrapMode);
        }

        public ITextureArray CreateTextureArrayFromBytes(
            int width,
            int height,
            int layers,
            byte[] imageBytes,
            TextureFormat textureFormat,
            MagFilter magFilter = MagFilter.Nearest,
            MinFilter minFilter = MinFilter.Nearest,
            WrapMode wrapMode = WrapMode.Repeat)
        {
            return new TextureArray(width, height, layers, imageBytes, textureFormat, magFilter, minFilter, wrapMode);
        }

        public IBuffer CreateBuffer<T>(int itemCount) where T: unmanaged
        {
            return new Buffer<T>(typeof(T).Name, itemCount);
        }

        public IBuffer CreateBuffer<T>(string name, int itemCount) where T: unmanaged
        {
            return new Buffer<T>(name, itemCount);
        }

        public IBuffer CreateBuffer<T>(T item) where T: unmanaged
        {
            return new Buffer<T>(typeof(T).Name, item);
        }

        public IBuffer CreateBuffer<T>(string name, T item) where T: unmanaged
        {
            return new Buffer<T>(name, item);
        }

        public IBuffer CreateBuffer<T>(T[] items) where T: unmanaged
        {
            return new Buffer<T>(typeof(T).Name, items);
        }

        public IBuffer CreateBuffer<T>(string name, T[] items) where T: unmanaged
        {
            return new Buffer<T>(name, items);
        }

        public IBuffer CreateBuffer<T>(IEnumerable<T> items) where T: unmanaged
        {
            return new Buffer<T>(typeof(T).Name, items.ToArray());
        }

        public IInputLayout GetInputLayout(VertexType vertexType)
        {
            return _inputLayoutProvider.GetInputLayout(vertexType);
        }

        public void SetViewport(int left, int top, int width, int height)
        {
            GL.Viewport(left, top, width, height);
        }

        private void DebugCallback(
            GL.DebugSource source,
            GL.DebugType type,
            uint id,
            GL.DebugSeverity severity,
            int length,
            IntPtr message,
            IntPtr userParam)
        {
            var messageString = Marshal.PtrToStringAnsi(message, length);

            switch (severity)
            {
                case GL.DebugSeverity.Notification or GL.DebugSeverity.DontCare:
                    _logger.Verbose("GL: {@Type} | {@MessageString}", type, messageString);
                    break;
                case GL.DebugSeverity.High:
                    _logger.Error("GL: {@Type} | {@MessageString}", type, messageString);
                    break;
                case GL.DebugSeverity.Medium:
                    _logger.Warning("GL: {@Type} | {@MessageString}", type, messageString);
                    break;
                case GL.DebugSeverity.Low:
                    _logger.Information("GL: {@Type} | {@MessageString}", type, messageString);
                    break;
            }

            if (type == GL.DebugType.Error)
            {
                _logger.Error("{@MessageString}", messageString);
#if DEBUG
                Debugger.Break();
#endif
            }
        }
    }
}
