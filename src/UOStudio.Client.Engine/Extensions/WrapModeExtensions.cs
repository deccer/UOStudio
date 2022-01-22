using System.Runtime.CompilerServices;
using UOStudio.Client.Engine.Graphics;
using UOStudio.Client.Engine.Native.OpenGL;

namespace UOStudio.Client.Engine.Extensions
{
    public static class WrapModeExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GL.TextureWrapMode ToTextureWrapMode(this WrapMode wrapMode)
        {
            return wrapMode switch
            {
                WrapMode.Clamp => GL.TextureWrapMode.Clamp,
                WrapMode.ClampToBorder => GL.TextureWrapMode.ClampToBorder,
                WrapMode.ClampToEdge => GL.TextureWrapMode.ClampToEdge,
                WrapMode.Repeat => GL.TextureWrapMode.Repeat,
                WrapMode.MirrorRepeat => GL.TextureWrapMode.MirroredRepeat
            };
        }
    }
}
