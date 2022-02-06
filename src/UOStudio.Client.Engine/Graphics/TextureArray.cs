using System.Runtime.InteropServices;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using UOStudio.Client.Engine.Extensions;
using UOStudio.Client.Engine.Native.OpenGL;

namespace UOStudio.Client.Engine.Graphics
{
    internal sealed class TextureArray : ITextureArray
    {
        private readonly uint _id;
        private readonly GL.PixelFormat _pixelFormat;
        private readonly GL.PixelType _pixelType;
        private readonly int _width;
        private readonly int _height;
        private readonly int _slices;

        public GL.SizedInternalFormat Format { get; private set; }

        private TextureArray()
        {
            _id = GL.CreateTexture(GL.TextureTarget.Texture2dArray);
        }

        internal TextureArray(
            int width,
            int height,
            int slices,
            byte[] imageBytes,
            TextureFormat textureFormat,
            MagFilter magFilter = MagFilter.Nearest,
            MinFilter minFilter = MinFilter.Nearest,
            WrapMode wrapMode = WrapMode.Repeat)
            : this()
        {
            _pixelFormat = textureFormat.ToPixelFormat();
            _pixelType = textureFormat.ToPixelType();
            _width = width;
            _height = height;
            _slices = slices;
            Format = textureFormat.ToSizedInternalFormat();
            var label = $"TA_{width}x{height}x{slices}_{textureFormat}_{minFilter}_{magFilter}_{wrapMode}";
            GL.ObjectLabel(GL.ObjectIdentifier.Texture, _id, label);
            GL.TextureStorage3D(_id, 1, Format, width, height, slices);
            GL.TextureParameter(_id, GL.TextureParameterName.TextureMinFilter, (int)minFilter.ToTextureMinFilter());
            GL.TextureParameter(_id, GL.TextureParameterName.TextureMagFilter, (int)magFilter.ToTextureMagFilter());
            GL.TextureParameter(_id, GL.TextureParameterName.TextureWrapS, (int)wrapMode.ToTextureWrapMode());
            GL.TextureParameter(_id, GL.TextureParameterName.TextureWrapT, (int)wrapMode.ToTextureWrapMode());
            GL.TextureSubImage3D(
                _id,
                0,
                0,
                0,
                0,
                width,
                height,
                slices,
                _pixelFormat,
                GL.PixelType.UnsignedByte,
                imageBytes);
        }

        internal TextureArray(
            string[] fileNames,
            TextureFormat textureFormat = TextureFormat.Rgb8,
            MagFilter magFilter = MagFilter.Linear,
            MinFilter minFilter = MinFilter.Linear,
            WrapMode wrapMode = WrapMode.Repeat)
            : this()
        {
            var layer = 0;
            _pixelFormat = textureFormat.ToPixelFormat();
            _pixelType = textureFormat.ToPixelType();
            _slices = fileNames.Length;
            foreach (var fileName in fileNames)
            {
                if (!File.Exists(fileName))
                {
                    continue;
                }

                using var imageStream = File.OpenRead(fileName);
                using var image = Image.Load<Rgba32>(imageStream);

                if (layer == 0)
                {
                    Format = textureFormat.ToSizedInternalFormat();
                    var label = $"TA_{image.Width}x{image.Height}x{fileNames.Length}_{textureFormat}_{minFilter}_{magFilter}_{wrapMode}";
                    GL.ObjectLabel(GL.ObjectIdentifier.Texture, _id, label);
                    GL.TextureStorage3D(_id, 1, Format, image.Width, image.Height, fileNames.Length);

                    GL.TextureParameter(_id, GL.TextureParameterName.TextureMinFilter, (int)minFilter.ToTextureMinFilter());
                    GL.TextureParameter(_id, GL.TextureParameterName.TextureMagFilter, (int)magFilter.ToTextureMagFilter());
                    GL.TextureParameter(_id, GL.TextureParameterName.TextureWrapS, (int)wrapMode.ToTextureWrapMode());
                    GL.TextureParameter(_id, GL.TextureParameterName.TextureWrapT, (int)wrapMode.ToTextureWrapMode());

                    _width = image.Width;
                    _height = image.Height;
                }

                UploadLayer(image, layer++, textureFormat);
            }
        }

        internal TextureArray(
            IEnumerable<Image> images,
            TextureFormat textureFormat = TextureFormat.Rgb8,
            MagFilter magFilter = MagFilter.Linear,
            MinFilter minFilter = MinFilter.Linear,
            WrapMode wrapMode = WrapMode.Repeat)
            : this()
        {
            var layer = 0;
            _pixelFormat = textureFormat.ToPixelFormat();
            _pixelType = textureFormat.ToPixelType();
            _slices = images.Count();

            foreach (var image in images)
            {
                if (layer == 0)
                {
                    Format = textureFormat.ToSizedInternalFormat();
                    var label = $"TA_{image.Width}x{image.Height}x{images.Count()}_{textureFormat}_{minFilter}_{magFilter}_{wrapMode}";
                    GL.ObjectLabel(GL.ObjectIdentifier.Texture, _id, label);
                    GL.TextureStorage3D(_id, 1, Format, image.Width, image.Height, images.Count());

                    GL.TextureParameter(_id, GL.TextureParameterName.TextureMinFilter, (int)minFilter.ToTextureMinFilter());
                    GL.TextureParameter(_id, GL.TextureParameterName.TextureMagFilter, (int)magFilter.ToTextureMagFilter());
                    GL.TextureParameter(_id, GL.TextureParameterName.TextureWrapS, (int)wrapMode.ToTextureWrapMode());
                    GL.TextureParameter(_id, GL.TextureParameterName.TextureWrapT, (int)wrapMode.ToTextureWrapMode());

                    _width = image.Width;
                    _height = image.Height;
                }

                UploadLayer((Image<Rgba32>)image, layer++, textureFormat);
            }
        }

        public void Bind(uint unit)
        {
            GL.BindTextureUnit(unit, _id);
        }

        public void Dispose()
        {
            GL.DeleteTexture(_id);
        }

        public byte[] GetBytes()
        {
            int byteCount = CalculateByteCount();
            var bytes = new byte[byteCount];

            GL.GetTextureImage(_id, 0, _pixelFormat, _pixelType, byteCount, ref bytes);

            return bytes;
        }

        public static implicit operator uint(TextureArray texture)
        {
            return texture._id;
        }

        private int CalculateByteCount()
        {
            var bytesPerPixel = _pixelType switch
            {
                GL.PixelType.Byte => 1,
                GL.PixelType.Float => 4,
                GL.PixelType.Int => 4,
                GL.PixelType.Short => 2,
                GL.PixelType.UnsignedByte => 1,
                GL.PixelType.UnsignedInt => 4,
                GL.PixelType.UnsignedShort => 2,
                _ => throw new UOStudioEngineException()
            };
            var componentCount = _pixelFormat switch
            {
                GL.PixelFormat.Bgr => 3,
                GL.PixelFormat.Bgra => 4,
                GL.PixelFormat.Red => 1,
                GL.PixelFormat.Green => 1,
                GL.PixelFormat.Blue => 1,
                GL.PixelFormat.Luminance => 1,
                GL.PixelFormat.Rg => 2,
                GL.PixelFormat.Rgb => 3,
                GL.PixelFormat.Rgba => 4,
                GL.PixelFormat.AbgrExt => 4,
                GL.PixelFormat.DepthComponent => 1,
                GL.PixelFormat.DepthStencil => 2,
                _ => throw new UOStudioEngineException()
            };

            return _width * _height * _slices * bytesPerPixel * componentCount;
        }

        private void UploadLayer(Image<Rgba32> image, int layer, TextureFormat textureFormat)
        {
            if (image.TryGetSinglePixelSpan(out var pixelSpan))
            {
                GL.TextureSubImage3D(_id, 0, 0, 0, layer,
                    image.Width,
                    image.Height,
                    1,
                    textureFormat.ToPixelFormat(),
                    GL.PixelType.UnsignedByte,
                    pixelSpan.GetPinnableReference());
            }
            else
            {
                Log.Logger.Error("Texture: :(");
            }
        }
    }
}
