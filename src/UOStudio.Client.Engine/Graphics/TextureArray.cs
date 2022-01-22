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
        
        private TextureArray()
        {
            _id = GL.CreateTexture(GL.TextureTarget.Texture2dArray);
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
                    var label = $"TA_{image.Width}x{image.Height}x{fileNames.Length}_{textureFormat}_{minFilter}_{magFilter}_{wrapMode}";
                    GL.ObjectLabel(GL.ObjectIdentifier.Texture, _id, label);
                    GL.TextureStorage3D(_id, 1, textureFormat.ToSizedInternalFormat(), image.Width, image.Height, fileNames.Length);

                    GL.TextureParameter(_id, GL.TextureParameterName.TextureMinFilter, (int)minFilter.ToTextureMinFilter());
                    GL.TextureParameter(_id, GL.TextureParameterName.TextureMagFilter, (int)magFilter.ToTextureMagFilter());
                    GL.TextureParameter(_id, GL.TextureParameterName.TextureWrapS, (int)wrapMode.ToTextureWrapMode());
                    GL.TextureParameter(_id, GL.TextureParameterName.TextureWrapT, (int)wrapMode.ToTextureWrapMode());
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
            foreach (var image in images)
            {
                if (layer == 0)
                {
                    var label = $"TA_{image.Width}x{image.Height}x{images.Count()}_{textureFormat}_{minFilter}_{magFilter}_{wrapMode}";
                    GL.ObjectLabel(GL.ObjectIdentifier.Texture, _id, label);
                    GL.TextureStorage3D(_id, 1, textureFormat.ToSizedInternalFormat(), image.Width, image.Height, images.Count());

                    GL.TextureParameter(_id, GL.TextureParameterName.TextureMinFilter, (int)minFilter.ToTextureMinFilter());
                    GL.TextureParameter(_id, GL.TextureParameterName.TextureMagFilter, (int)magFilter.ToTextureMagFilter());
                    GL.TextureParameter(_id, GL.TextureParameterName.TextureWrapS, (int)wrapMode.ToTextureWrapMode());
                    GL.TextureParameter(_id, GL.TextureParameterName.TextureWrapT, (int)wrapMode.ToTextureWrapMode());
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