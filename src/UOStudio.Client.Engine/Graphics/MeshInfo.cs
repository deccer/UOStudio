using UOStudio.Client.Engine.Mathematics;

namespace UOStudio.Client.Engine.Graphics
{
    public sealed class MeshInfo
    {
        public int IndexCount { get; }

        public int IndexOffset { get; }

        public int VertexCount { get; }

        public int VertexOffset { get; }

        public Matrix Transform { get; }

        public MeshInfo(
            int vertexCount,
            int vertexOffset,
            int indexCount,
            int indexOffset,
            Matrix transform)
        {
            VertexCount = vertexCount;
            VertexOffset = vertexOffset;
            IndexCount = indexCount;
            IndexOffset = indexOffset;
            Transform = transform;
        }
    }
}
