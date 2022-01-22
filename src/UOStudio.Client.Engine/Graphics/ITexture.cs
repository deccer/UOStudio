namespace UOStudio.Client.Engine.Graphics
{
    public interface ITexture : IDisposable
    {
        int Width { get; set; }

        int Height { get; set; }

        void Bind(uint textureUnit);

        IntPtr AsIntPtr();
    }
}