using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Extensions.Configuration;
using Serilog;
using UOStudio.Client.Engine;
using UOStudio.Client.Engine.Graphics;
using UOStudio.Tools.TextureAtlasGenerator.Abstractions;

namespace UOStudio.Tools.TextureAtlasGenerator
{
    internal sealed class Texture3dGenerator : ITexture3dGenerator
    {
        private readonly ILogger _logger;
        private readonly IApplication _game;
        private readonly IGraphicsDevice _graphicsDevice;
        private readonly int _atlasPageSize;

        public Texture3dGenerator(
            ILogger logger,
            IConfiguration configuration)
        {
            _logger = logger.ForContext<Texture3dGenerator>();
            _atlasPageSize = int.TryParse(configuration["AtlasPageSize"], out var atlasPageSize)
                ? atlasPageSize
                : 2048;

            /*
            _game = new Game();
            _graphicsDevice = gdm.GraphicsDevice;
            */
        }

        public byte[] Generate3dTexture(IEnumerable<Bitmap> atlasPages)
        {
            /*
            var textures = atlasPages
                .Select(atlasPage => BitmapToTexture2D(_graphicsDevice, atlasPage))
                .ToArray();

            var pageCount = textures.Length;
            var texture3DData = new Color[pageCount * _atlasPageSize * _atlasPageSize];
            for (var pageIndex = 0; pageIndex < pageCount; pageIndex++)
            {
                var pageData = new Color[_atlasPageSize * _atlasPageSize];
                var page = textures[pageIndex];
                page.GetData(pageData);
                for (var y = 0; y < _atlasPageSize; y++)
                {
                    for (var x = 0; x < _atlasPageSize; x++)
                    {
                        var texture2DIndex = y * _atlasPageSize + x;
                        var texture3DIndex = pageIndex * _atlasPageSize * _atlasPageSize + texture2DIndex;
                        texture3DData[texture3DIndex] = pageData[texture2DIndex];
                    }
                }

                page.Dispose();
            }

            return ConvertColorsToBytes(texture3DData);
            */

            throw new NotImplementedException();
        }

        public void Dispose()
        {
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
