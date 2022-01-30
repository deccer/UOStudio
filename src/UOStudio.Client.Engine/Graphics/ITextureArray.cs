namespace UOStudio.Client.Engine.Graphics
{
    public interface ITextureArray : IDisposable
    {
        void Bind(uint unit);

        byte[] GetBytes();
    }
}
