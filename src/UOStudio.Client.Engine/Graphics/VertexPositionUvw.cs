using System.Runtime.InteropServices;
using UOStudio.Client.Engine.Mathematics;

namespace UOStudio.Client.Engine.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct VertexPositionUvw
    {
        public VertexPositionUvw(Vector3 position, Vector3 uvw)
        {
            Position = position;
            Uvw = uvw;
        }

        public readonly Vector3 Position;

        public readonly Vector3 Uvw;
    }
}
