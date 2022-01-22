﻿using CSharpFunctionalExtensions;
using SixLabors.ImageSharp;

namespace UOStudio.Client.Engine.Graphics
{
    public interface IGraphicsDevice : IDisposable
    {
        void Clear(UOStudio.Client.Engine.Mathematics.Color clearColor);

        Result<IShader> CreateShaderProgramFromFiles(
            string label,
            string vertexShaderFileName,
            string fragmentShaderFileName);

        Result<IShader> CreateShaderProgramFromSources(
            string label,
            string vertexShaderSource,
            string fragmentShaderSource);

        IFramebuffer CreateFramebuffer(
            string label,
            ITexture[] colorAttachments,
            ITexture depthAttachment = null);

        ITexture CreateDepthTexture(
            int width,
            int height,
            TextureFormat textureFormat);

        ITexture CreateTexture(
            string label,
            int width,
            int height,
            TextureFormat textureFormat,
            MagFilter magFilter = MagFilter.Nearest,
            MinFilter minFilter = MinFilter.Nearest,
            WrapMode wrapMode = WrapMode.Repeat,
            IntPtr? initialData = null);

        ITexture CreateTextureFromFile(
            string fileName,
            MagFilter magFilter = MagFilter.Linear,
            MinFilter minFilter = MinFilter.Linear,
            WrapMode wrapMode = WrapMode.Repeat);

        ITextureArray CreateTextureArrayFromImages(
            IEnumerable<Image> images,
            TextureFormat textureFormat,
            MagFilter magFilter = MagFilter.Nearest,
            MinFilter minFilter = MinFilter.Nearest,
            WrapMode wrapMode = WrapMode.Repeat);

        IBuffer CreateBuffer<T>(int itemCount) where T : unmanaged;

        IBuffer CreateBuffer<T>(T item) where T: unmanaged;

        IBuffer CreateBuffer<T>(string name, T item) where T: unmanaged;

        IBuffer CreateBuffer<T>(string name, int itemCount) where T: unmanaged;

        IBuffer CreateBuffer<T>(T[] items) where T: unmanaged;

        IBuffer CreateBuffer<T>(string name, T[] items) where T: unmanaged;

        IBuffer CreateBuffer<T>(IEnumerable<T> items) where T: unmanaged;

        IInputLayout GetInputLayout(VertexType vertexType);

        void SetViewport(int left, int top, int width, int height);
    }
}