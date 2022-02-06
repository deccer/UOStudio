using UOStudio.Client.Engine.Native.OpenGL;

namespace UOStudio.Client.Engine.Graphics
{
    public sealed class TextureView : ITextureView
    {
        private readonly uint _id;

        public TextureView(ITextureArray textureArray, uint layer)
        {
            _id = GL.GenTexture();
            GL.TextureView(_id, GL.TextureTarget.Texture2d, (TextureArray)textureArray, textureArray.Format, 0, 1, layer, 1);
        }

        public void Dispose()
        {
            GL.DeleteTexture(_id);
        }

        public IntPtr AsIntPtr()
        {
            return new IntPtr(_id);
        }
    }
}
