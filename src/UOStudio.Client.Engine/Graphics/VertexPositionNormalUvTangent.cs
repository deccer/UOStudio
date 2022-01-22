using System.Runtime.InteropServices;
using UOStudio.Client.Engine.Mathematics;

namespace UOStudio.Client.Engine.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct VertexPositionNormalUvTangent
    {
        public VertexPositionNormalUvTangent(Vector3 position, Vector3 normal, Vector2 uv, Vector4 tangent)
        {
            Position = position;
            Normal = normal;
            Uv = uv;
            Tangent = tangent;
        }

        public readonly Vector3 Position;

        public readonly Vector3 Normal;

        public readonly Vector2 Uv;

        public readonly Vector4 Tangent;
    }
}
