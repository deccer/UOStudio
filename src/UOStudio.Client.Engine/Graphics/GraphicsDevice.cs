using CSharpFunctionalExtensions;
using SixLabors.ImageSharp;

namespace UOStudio.Client.Engine.Graphics
{
    internal sealed class GraphicsDevice : IGraphicsDevice
    {
        private readonly IInputLayoutProvider _inputLayoutProvider;

        public GraphicsDevice(
            IInputLayoutProvider inputLayoutProvider)
        {
            _inputLayoutProvider = inputLayoutProvider;
        }

        public void Dispose()
        {
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
    }
}