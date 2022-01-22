using System.Runtime.InteropServices;
using UOStudio.Client.Engine.Mathematics;

namespace UOStudio.Client.Engine.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct VertexPositionColorNormalUv
    {
        public VertexPositionColorNormalUv(
            Vector3 position,
            Vector3 color,
            Vector3 normal,
            Vector2 uv)
        {
            Position = position;
            Color = color;
            Normal = normal;
            Uv = uv;
        }

        public readonly Vector3 Position;

        public readonly Vector3 Color;

        public readonly Vector3 Normal;

        public readonly Vector2 Uv;
    }
}
