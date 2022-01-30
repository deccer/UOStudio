namespace UOStudio.Client
{
    public interface ITextureAtlasProvider
    {
        bool TryLoadTextureAtlas(string atlasName, out ITextureAtlas textureAtlas);
    }
}
