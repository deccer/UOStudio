namespace UOStudio.Client.Engine.Graphics
{
    public interface IInputLayoutProvider : IDisposable
    {
        IInputLayout GetInputLayout(VertexType vertexType);
    }
}
