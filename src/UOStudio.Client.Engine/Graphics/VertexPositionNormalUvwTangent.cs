using System.Runtime.InteropServices;
using UOStudio.Client.Engine.Mathematics;

namespace UOStudio.Client.Engine.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct VertexPositionNormalUvwTangent
    {
        public VertexPositionNormalUvwTangent(Vector3 position, Vector3 normal, Vector3 uvw, Vector4 tangent)
        {
            Position = position;
            Normal = normal;
            Uvw = uvw;
            Tangent = tangent;
            _padding = Vector2.Zero;
        }

        public readonly Vector3 Position;

        public readonly Vector3 Normal;

        public readonly Vector3 Uvw;

        public readonly Vector4 Tangent;

        public readonly Vector2 _padding;
    }
}
