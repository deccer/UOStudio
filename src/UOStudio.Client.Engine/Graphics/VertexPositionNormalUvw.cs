using System.Runtime.InteropServices;
using UOStudio.Client.Engine.Mathematics;

namespace UOStudio.Client.Engine.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct VertexPositionNormalUvw
    {
        public VertexPositionNormalUvw(Vector3 position, Vector3 normal, Vector3 uvw)
        {
            Position = position;
            Normal = normal;
            Uvw = uvw;
        }

        public readonly Vector3 Position;

        public readonly Vector3 Normal;

        public readonly Vector3 Uvw;
    }
}
