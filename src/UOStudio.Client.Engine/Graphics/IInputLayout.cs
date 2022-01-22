namespace UOStudio.Client.Engine.Graphics
{
    public interface IInputLayout : IDisposable
    {
        void AddVertexBuffer(
            IBuffer vertexBuffer,
            int bindingIndex);

        void AddElementBuffer(IBuffer elementBuffer);

        void Bind();
    }
}