namespace UOStudio.Client.Engine.Graphics
{
    public interface IInputLayoutMapper
    {
        IReadOnlyCollection<VertexAttribute> MapVertexType(VertexType vertexType);
    }
}
