using System.Runtime.InteropServices;
using UOStudio.Client.Engine.Mathematics;

namespace UOStudio.Client.Engine.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct VertexPosition
    {
        public VertexPosition(Vector3 position)
        {
            Position = position;
        }

        public readonly Vector3 Position;
    }
}
