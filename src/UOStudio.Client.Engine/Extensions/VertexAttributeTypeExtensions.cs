using System.Runtime.CompilerServices;
using UOStudio.Client.Engine.Graphics;
using UOStudio.Client.Engine.Native.OpenGL;

namespace UOStudio.Client.Engine.Extensions
{
    public static class VertexAttributeTypeExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GL.VertexAttribType ToVertexAttribType(this VertexAttributeType vertexAttributeType)
        {
            return vertexAttributeType switch
            {
                VertexAttributeType.Byte => GL.VertexAttribType.Byte,
                VertexAttributeType.Float => GL.VertexAttribType.Float,
                VertexAttributeType.UnsignedByte => GL.VertexAttribType.UnsignedByte
            };
        }
    }
}
