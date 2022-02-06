using UOStudio.Client.Engine.Graphics;

namespace UOStudio.Client
{
    public interface ITextureAtlasProvider
    {
        bool TryLoadTextureAtlas(string atlasName, TextureFormat textureFormat, out ITextureAtlas textureAtlas);
    }
}
