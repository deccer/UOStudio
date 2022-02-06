using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using UOStudio.Client.Engine.Extensions;
using UOStudio.Client.Engine.Mathematics;
using UOStudio.Client.Engine.Native.OpenGL;

namespace UOStudio.Client.Engine.Graphics
{
    internal sealed class Texture : ITexture
    {
        private readonly uint _id;

        public static ITexture CreateFromFile(
            string fileName,
            MinFilter minFilter = MinFilter.Linear,
            MagFilter magFilter = MagFilter.Linear,
            WrapMode wrapMode = WrapMode.Repeat)
        {
            return new Texture(fileName, minFilter, magFilter, wrapMode);
        }

        public TextureFormat Format { get; private set; }

        public int Width { get; set; }

        public int Height { get; set; }

        private Texture()
        {
            _id = GL.CreateTexture(GL.TextureTarget.Texture2d);
        }

        internal Texture(
            int width,
            int height,
            TextureFormat textureFormat = TextureFormat.Rgb8,
            MinFilter minFilter = MinFilter.Linear,
            MagFilter magFilter = MagFilter.Linear,
            WrapMode wrapMode = WrapMode.Repeat,
            IReadOnlyList<Vector4> initialData = null,
            string name = "")
            : this()
        {
            Width = width;
            Height = height;
            Format = textureFormat;
            var label = string.IsNullOrEmpty(name)
                ? $"T_{width}x{height}_{textureFormat}_{minFilter}_{magFilter}_{wrapMode}"
                : $"T_{name}_{width}x{height}_{textureFormat}_{minFilter}_{magFilter}_{wrapMode}";
            var sizedInternalFormat = textureFormat.ToSizedInternalFormat();

            GL.ObjectLabel(GL.ObjectIdentifier.Texture, _id, label);
            var mipLevels = (int)MathF.Floor(MathF.Log2(MathF.Max(Width, Height)));
            GL.TextureStorage2D(_id, mipLevels, sizedInternalFormat, width, height);
            GL.TextureParameter(_id, GL.TextureParameterName.TextureMinFilter, (int)minFilter.ToTextureMinFilter());
            GL.TextureParameter(_id, GL.TextureParameterName.TextureMagFilter, (int)magFilter.ToTextureMagFilter());
            GL.TextureParameter(_id, GL.TextureParameterName.TextureWrapS, (int)wrapMode.ToTextureWrapMode());
            GL.TextureParameter(_id, GL.TextureParameterName.TextureWrapT, (int)wrapMode.ToTextureWrapMode());
            GL.TextureParameter(_id, GL.TextureParameterName.TextureBaseLevel, 0);
            GL.TextureParameter(_id, GL.TextureParameterName.TextureMaxLevel, mipLevels);

            if (initialData != null)
            {
                GL.TextureSubImage2D(_id, 0, 0, 0, width, height,
                    textureFormat.ToPixelFormat(),
                    textureFormat.ToPixelType(),
                    initialData[0]);
            }

            if (!IsDepthFormat(sizedInternalFormat) && mipLevels > 1)
            {
                GL.GenerateTextureMipmap(_id);
            }
        }

        internal Texture(
            int width,
            int height,
            TextureFormat textureFormat,
            MinFilter minFilter,
            MagFilter magFilter,
            WrapMode wrapMode,
            IntPtr initialData,
            string name = "")
            : this()
        {
            Width = width;
            Height = height;
            Format = textureFormat;
            var label = string.IsNullOrEmpty(name)
                ? $"T_{width}x{height}_{textureFormat}_{minFilter}_{magFilter}_{wrapMode}"
                : $"T_{name}_{width}x{height}_{textureFormat}_{minFilter}_{magFilter}_{wrapMode}";
            var sizedInternalFormat = textureFormat.ToSizedInternalFormat();

            GL.ObjectLabel(GL.ObjectIdentifier.Texture, _id, label);
            var mipLevels = (int)MathF.Floor(MathF.Log2(MathF.Max(Width, Height)));
            GL.TextureStorage2D(_id, mipLevels, sizedInternalFormat, width, height);
            GL.TextureParameter(_id, GL.TextureParameterName.TextureMinFilter, (int)minFilter.ToTextureMinFilter());
            GL.TextureParameter(_id, GL.TextureParameterName.TextureMagFilter, (int)magFilter.ToTextureMagFilter());
            GL.TextureParameter(_id, GL.TextureParameterName.TextureWrapS, (int)wrapMode.ToTextureWrapMode());
            GL.TextureParameter(_id, GL.TextureParameterName.TextureWrapT, (int)wrapMode.ToTextureWrapMode());
            if (!IsDepthFormat(sizedInternalFormat) && mipLevels > 1)
            {
                GL.TextureParameter(_id, GL.TextureParameterName.TextureBaseLevel, 0);
                GL.TextureParameter(_id, GL.TextureParameterName.TextureMaxLevel, mipLevels);
            }
            else
            {
                var borderColor = Vector4.One;
                GL.TextureParameter(_id, GL.TextureParameterName.TextureBorderColor, borderColor.X);
            }

            if (initialData != IntPtr.Zero)
            {
                GL.TextureSubImage2D(_id, 0, 0, 0, width, height, textureFormat.ToPixelFormat(), textureFormat.ToPixelType(), initialData);
            }

            if (!IsDepthFormat(sizedInternalFormat) && mipLevels > 1)
            {
                GL.GenerateTextureMipmap(_id);
            }
        }

        internal Texture(
            Stream imageStream,
            MinFilter minFilter = MinFilter.Nearest,
            MagFilter magFilter = MagFilter.Nearest,
            WrapMode wrapMode = WrapMode.Repeat)
            : this()
        {
            using var image = Image.Load<Rgba32>(imageStream);

            var label = $"T_{image.Width}x{image.Height}_{minFilter}_{magFilter}_{wrapMode}";
            GL.ObjectLabel(GL.ObjectIdentifier.Texture, _id, label);

            CreateTexture(image, minFilter, magFilter, wrapMode);
        }

        internal Texture(
            string fileName,
            MinFilter minFilter = MinFilter.Linear,
            MagFilter magFilter = MagFilter.Linear,
            WrapMode wrapMode = WrapMode.Repeat)
            : this()
        {
            using var image = Image.Load<Rgba32>(fileName);

            var label = $"T_{image.Width}x{image.Height}_{Path.GetFileName(fileName)}_{minFilter}_{magFilter}_{wrapMode}";
            GL.ObjectLabel(GL.ObjectIdentifier.Texture, _id, label);

            CreateTexture(image, minFilter, magFilter, wrapMode);
        }

        public IntPtr AsIntPtr()
        {
            return new IntPtr(_id);
        }

        public static implicit operator uint(Texture texture)
        {
            return texture._id;
        }

        public static implicit operator IntPtr(Texture texture)
        {
            return (IntPtr)texture._id;
        }

        public void Dispose()
        {
            GL.DeleteTexture(_id);
        }

        public void Bind(uint textureUnit)
        {
            GL.BindTextureUnit(textureUnit, _id);
        }

        private static bool IsDepthFormat(GL.SizedInternalFormat sizedInternalFormat)
        {
            return sizedInternalFormat switch
            {
                GL.SizedInternalFormat.DepthComponent16 => true,
                GL.SizedInternalFormat.DepthComponent24 => true,
                GL.SizedInternalFormat.DepthComponent32 => true,
                GL.SizedInternalFormat.DepthComponent32f => true,
                GL.SizedInternalFormat.DepthComponent32fNv => true,
                GL.SizedInternalFormat.Depth24Stencil8 => true,
                GL.SizedInternalFormat.Depth32fStencil8 => true,
                GL.SizedInternalFormat.Depth32fStencil8Nv => true,
                _ => false
            };
        }

        private void CreateTexture(
            Image<Rgba32> image,
            MinFilter minFilter,
            MagFilter magFilter,
            WrapMode wrapMode)
        {
            Width = image.Width;
            Height = image.Height;

            image.Mutate(ipc => ipc.Flip(FlipMode.Vertical));
            if (image.TryGetSinglePixelSpan(out var pixelSpan))
            {
                var mipLevels = (int)MathF.Floor(MathF.Log2(MathF.Max(Width, Height)));
                var sizedInternalFormat = GL.SizedInternalFormat.Rgba8;
                Format = sizedInternalFormat.ToTextureFormat();
                GL.TextureStorage2D(_id, mipLevels, sizedInternalFormat, image.Width, image.Height);
                GL.TextureParameter(_id, GL.TextureParameterName.TextureMinFilter, (int)minFilter.ToTextureMinFilter());
                GL.TextureParameter(_id, GL.TextureParameterName.TextureMagFilter, (int)magFilter.ToTextureMagFilter());
                GL.TextureParameter(_id, GL.TextureParameterName.TextureWrapS, (int)wrapMode.ToTextureWrapMode());
                GL.TextureParameter(_id, GL.TextureParameterName.TextureWrapT, (int)wrapMode.ToTextureWrapMode());
                GL.TextureParameter(_id, GL.TextureParameterName.TextureBaseLevel, 0);
                GL.TextureParameter(_id, GL.TextureParameterName.TextureMaxLevel, mipLevels);

                GL.TextureSubImage2D(_id, 0, 0, 0,
                    image.Width,
                    image.Height,
                    GL.PixelFormat.Rgba,
                    GL.PixelType.UnsignedByte,
                    pixelSpan.GetPinnableReference());
                if (mipLevels > 1)
                {
                    GL.GenerateTextureMipmap(_id);
                }
            }
            else
            {
                Log.Logger.Error("Texture: :(");
            }
        }
    }
}
