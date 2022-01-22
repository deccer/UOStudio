using System.Runtime.InteropServices;
using UOStudio.Client.Engine.Mathematics;

namespace UOStudio.Client.Engine.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct VertexPositionColorNormal
    {
        public VertexPositionColorNormal(
            Vector3 position,
            Vector3 color,
            Vector3 normal)
        {
            Position = position;
            Color = color;
            Normal = normal;
        }

        public readonly Vector3 Position;

        public readonly Vector3 Color;

        public readonly Vector3 Normal;
    }
}