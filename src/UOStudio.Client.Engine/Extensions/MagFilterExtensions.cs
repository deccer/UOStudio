using System.Runtime.CompilerServices;
using UOStudio.Client.Engine.Graphics;
using UOStudio.Client.Engine.Native.OpenGL;

namespace UOStudio.Client.Engine.Extensions
{
    public static class MagFilterExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GL.TextureMagFilter ToTextureMagFilter(this MagFilter magFilter)
        {
            return magFilter switch
            {
                MagFilter.Linear => GL.TextureMagFilter.Linear,
                MagFilter.Nearest => GL.TextureMagFilter.Nearest,
            };
        }
    }
}
