using System.Runtime.CompilerServices;
using UOStudio.Client.Engine.Graphics;
using UOStudio.Client.Engine.Native.OpenGL;

namespace UOStudio.Client.Engine.Extensions
{
    public static class TextureFormatExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GL.SizedInternalFormat ToSizedInternalFormat(this TextureFormat textureFormat)
        {
            return textureFormat switch
            {
                TextureFormat.Depth16 => GL.SizedInternalFormat.DepthComponent16,
                TextureFormat.Depth24 => GL.SizedInternalFormat.DepthComponent24,
                TextureFormat.Depth24Stencil8 => GL.SizedInternalFormat.Depth24Stencil8,
                TextureFormat.Depth32f => GL.SizedInternalFormat.DepthComponent32f,
                TextureFormat.Depth32fStencil8 => GL.SizedInternalFormat.Depth32fStencil8,
                TextureFormat.R8 => GL.SizedInternalFormat.R8,
                TextureFormat.Rg8 => GL.SizedInternalFormat.Rg8,
                TextureFormat.Rgb8 => GL.SizedInternalFormat.Rgb8,
                TextureFormat.Rgb10a2 => GL.SizedInternalFormat.Rgb10A2,
                TextureFormat.Rgba8 => GL.SizedInternalFormat.Rgba8,
                TextureFormat.R16f => GL.SizedInternalFormat.R16f,
                TextureFormat.Rg16f => GL.SizedInternalFormat.Rg16f,
                TextureFormat.Rgb16f => GL.SizedInternalFormat.Rgb16f,
                TextureFormat.Rgba16f => GL.SizedInternalFormat.Rgba16f,
                TextureFormat.R32f => GL.SizedInternalFormat.R32f,
                TextureFormat.Rg32f => GL.SizedInternalFormat.Rg32f,
                TextureFormat.Rgb32f => GL.SizedInternalFormat.Rgb32f,
                TextureFormat.Rgba32f => GL.SizedInternalFormat.Rgba32f,
                TextureFormat.R16ui => GL.SizedInternalFormat.R16ui,
                TextureFormat.Rg16ui => GL.SizedInternalFormat.Rg16ui,
                TextureFormat.Rg11b10f => GL.SizedInternalFormat.R11fG11fB10f,
            };
        }

        public static GL.PixelType ToPixelType(this TextureFormat textureFormat)
        {
            return textureFormat switch
            {
                TextureFormat.Depth16 => GL.PixelType.Float,
                TextureFormat.Depth24 => GL.PixelType.Float,
                TextureFormat.Depth24Stencil8 => GL.PixelType.Float,
                TextureFormat.Depth32f => GL.PixelType.Float,
                TextureFormat.Depth32fStencil8 => GL.PixelType.Float,
                TextureFormat.R8 => GL.PixelType.UnsignedByte,
                TextureFormat.Rg8 => GL.PixelType.UnsignedByte,
                TextureFormat.Rgb8 => GL.PixelType.UnsignedByte,
                TextureFormat.Rgb10a2 => GL.PixelType.UnsignedInt1010102,
                TextureFormat.Rgba8 => GL.PixelType.UnsignedByte,
                TextureFormat.R16f => GL.PixelType.Float,
                TextureFormat.Rg16f => GL.PixelType.Float,
                TextureFormat.Rgb16f => GL.PixelType.Float,
                TextureFormat.Rgba16f => GL.PixelType.Float,
                TextureFormat.R32f => GL.PixelType.Float,
                TextureFormat.Rg32f => GL.PixelType.Float,
                TextureFormat.Rgb32f => GL.PixelType.Float,
                TextureFormat.Rgba32f => GL.PixelType.Float,
                TextureFormat.R16ui => GL.PixelType.UnsignedInt,
                TextureFormat.Rg16ui => GL.PixelType.UnsignedInt,
                TextureFormat.Rg11b10f => GL.PixelType.Float,
            };
        }

        public static GL.PixelFormat ToPixelFormat(this TextureFormat textureFormat)
        {
            return textureFormat switch
            {
                TextureFormat.Depth16 => GL.PixelFormat.DepthComponent,
                TextureFormat.Depth24 => GL.PixelFormat.DepthComponent,
                TextureFormat.Depth24Stencil8 => GL.PixelFormat.DepthStencil,
                TextureFormat.Depth32f => GL.PixelFormat.DepthComponent,
                TextureFormat.Depth32fStencil8 => GL.PixelFormat.DepthStencil,
                TextureFormat.R8 => GL.PixelFormat.Red,
                TextureFormat.Rg8 => GL.PixelFormat.Rg,
                TextureFormat.Rgb8 => GL.PixelFormat.Rgb,
                TextureFormat.Rgb10a2 => GL.PixelFormat.Rgba,
                TextureFormat.Rgba8 => GL.PixelFormat.Rgba,
                TextureFormat.R16f => GL.PixelFormat.Red,
                TextureFormat.Rg16f => GL.PixelFormat.Rg,
                TextureFormat.Rgb16f => GL.PixelFormat.Rgb,
                TextureFormat.Rgba16f => GL.PixelFormat.Rgba,
                TextureFormat.R32f => GL.PixelFormat.Red,
                TextureFormat.Rg32f => GL.PixelFormat.Rg,
                TextureFormat.Rgb32f => GL.PixelFormat.Rgb,
                TextureFormat.Rgba32f => GL.PixelFormat.Rgba,
                TextureFormat.R16ui => GL.PixelFormat.RedInteger,
                TextureFormat.Rg16ui => GL.PixelFormat.RedInteger,
                TextureFormat.Rg11b10f => GL.PixelFormat.Rgb,
            };
        }
    }
}
