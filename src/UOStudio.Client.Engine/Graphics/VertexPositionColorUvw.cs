using System.Runtime.InteropServices;
using UOStudio.Client.Engine.Mathematics;

namespace UOStudio.Client.Engine.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct VertexPositionColorUvw
    {
        public VertexPositionColorUvw(
            Vector3 position,
            Vector3 color,
            Vector3 uvw)
        {
            Position = position;
            Color = color;
            Uvw = uvw;
        }

        public readonly Vector3 Position;

        public readonly Vector3 Color;

        public readonly Vector3 Uvw;
    }
}
