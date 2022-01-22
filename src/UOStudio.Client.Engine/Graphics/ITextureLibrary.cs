namespace UOStudio.Client.Engine.Graphics
{
    public interface ITextureLibrary : IDisposable
    {
        void AddTextureFromFile(string name, string filePath);

        void LoadResources(
            out IEnumerable<ITextureArray> textureArrays,
            out IDictionary<string, (int ArrayIndex, int TextureIndex)> indicesPerTextureName);
    }
}