using System.Runtime.CompilerServices;
using UOStudio.Client.Engine.Graphics;
using UOStudio.Client.Engine.Native.OpenGL;

namespace UOStudio.Client.Engine.Extensions
{
    public static class MinFilterExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GL.TextureMinFilter ToTextureMinFilter(this MinFilter minFilter)
        {
            return minFilter switch
            {
                MinFilter.Linear => GL.TextureMinFilter.Linear,
                MinFilter.LinearMipmapLinear => GL.TextureMinFilter.LinearMipmapLinear,
                MinFilter.LinearMipmapNearest => GL.TextureMinFilter.LinearMipmapNearest,
                MinFilter.Nearest => GL.TextureMinFilter.Nearest,
                MinFilter.NearestMipmapLinear => GL.TextureMinFilter.NearestMipmapLinear,
                MinFilter.NearestMipmapNearest => GL.TextureMinFilter.NearestMipmapNearest
            };
        }
    }
}
