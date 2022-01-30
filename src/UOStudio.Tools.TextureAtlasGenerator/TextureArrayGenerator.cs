using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Serilog;
using UOStudio.Client.Engine;
using UOStudio.Client.Engine.Graphics;
using UOStudio.Client.Engine.Native;
using UOStudio.Tools.TextureAtlasGenerator.Abstractions;

namespace UOStudio.Tools.TextureAtlasGenerator
{
    internal sealed class TextureArrayGenerator : ITextureArrayGenerator
    {
        private readonly ILogger _logger;
        private readonly IWindowFactory _windowFactory;
        private readonly IGraphicsDevice _graphicsDevice;
        private readonly int _atlasPageSize;

        public TextureArrayGenerator(
            ILogger logger,
            IWindowFactory windowFactory,
            IGraphicsDevice graphicsDevice,
            IConfiguration configuration)
        {
            _logger = logger.ForContext<TextureArrayGenerator>();
            _windowFactory = windowFactory;
            _graphicsDevice = graphicsDevice;

            _atlasPageSize = int.TryParse(configuration["AtlasPageSize"], out var atlasPageSize)
                ? atlasPageSize
                : 2048;
        }

        public byte[] GenerateTextureArray(IEnumerable<Bitmap> atlasPages)
        {
            if (!Sdl.Init(Sdl.InitFlags.Video))
            {
                return null;
            }

            var windowSettings = new WindowSettings
            {
                IsVsyncEnabled = true,
                ResolutionHeight = 10,
                ResolutionWidth = 10,
                Visible = false
            };

            var window = _windowFactory.CreateWindow("UOStudio.TextureGenerator", windowSettings);
            if (!window.Initialize())
            {
                return null;
            }

            if (!_graphicsDevice.Initialize(ContextSettings.Default, window.Handle))
            {
                return null;
            }

            var textures = atlasPages
                .Select(atlasPage => ToImageSharpImage(atlasPage))
                .ToArray();

            var textureArray = _graphicsDevice.CreateTextureArrayFromImages(textures, TextureFormat.Rgb8);
            var bytes = textureArray.GetBytes();
            textureArray.Dispose();

            window.Dispose();
            Sdl.Quit();

            return bytes;
        }

        public void Dispose()
        {
        }

        private static SixLabors.ImageSharp.Image ToImageSharpImage(Bitmap bitmap)
        {
            using var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            memoryStream.Seek(0, SeekOrigin.Begin);

            return SixLabors.ImageSharp.Image.Load(memoryStream);
        }

        private static ITexture BitmapToTexture2D(IGraphicsDevice graphicsDevice, Bitmap atlasPage)
        {
            /*
            var texture2D = new Texture2D(graphicsDevice, atlasPage.Width, atlasPage.Height, false, SurfaceFormat.Color);
            var bitmapData = atlasPage.LockBits(new Rectangle(0, 0, atlasPage.Width, atlasPage.Height),
                ImageLockMode.ReadOnly, atlasPage.PixelFormat);
            var pixelBufferSize = bitmapData.Height * bitmapData.Stride;
            var pixelBuffer = new byte[pixelBufferSize];
            Marshal.Copy(bitmapData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            texture2D.SetData(pixelBuffer);
            atlasPage.UnlockBits(bitmapData);
            atlasPage.Dispose();
            return texture2D;
            */

            throw new NotImplementedException();
        }

        private static byte[] ConvertColorsToBytes(Color[] data)
        {
            var n = data.Length;
            var bytes = new byte[n * sizeof(uint)];
            if (n == 0)
            {
                return bytes;
            }

            unsafe
            {
                fixed (byte* bytesPtr = &bytes[0])
                {
                    var targetArray = (uint*)bytesPtr;
                    for (var i = 0; i < n; i++)
                    {
                        targetArray[i] = data[i].B | ((uint)data[i].G << 8) | ((uint)data[i].R << 16) | ((uint)data[i].A << 24);
                    }
                }
            }

            return bytes;
        }
    }
}
