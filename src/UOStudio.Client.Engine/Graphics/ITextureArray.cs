using UOStudio.Client.Engine.Native.OpenGL;

namespace UOStudio.Client.Engine.Graphics
{
    public interface ITextureArray : IDisposable
    {
        void Bind(uint unit);

        byte[] GetBytes();

        GL.SizedInternalFormat Format { get; }
    }
}
