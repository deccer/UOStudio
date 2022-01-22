using System.Runtime.InteropServices;
using UOStudio.Client.Engine.Mathematics;

namespace UOStudio.Client.Engine.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct VertexPositionColorUv
    {
        public VertexPositionColorUv(
            Vector3 position,
            Vector3 color,
            Vector2 uv)
        {
            Position = position;
            Color = color;
            Uv = uv;
        }

        public readonly Vector3 Position;

        public readonly Vector3 Color;

        public readonly Vector2 Uv;
    }
}
