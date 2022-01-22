namespace UOStudio.Client.Engine.Graphics
{
    public readonly struct VertexAttribute
    {
        public readonly string Name;
        public readonly uint Index;
        public readonly VertexAttributeType Type;
        public readonly int Components;
        public readonly uint Offset;

        public VertexAttribute(string name, uint index, VertexAttributeType type, int components, uint offset)
        {
            Name = name;
            Index = index;
            Type = type;
            Components = components;
            Offset = offset;
        }
    }
}
