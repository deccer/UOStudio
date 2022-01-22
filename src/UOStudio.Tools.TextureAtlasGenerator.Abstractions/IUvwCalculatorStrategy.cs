using UOStudio.TextureAtlasGenerator.Contracts;

namespace UOStudio.TextureAtlasGenerator.Abstractions
{
    public interface IUvwCalculatorStrategy
    {
        Uvws CalculateUvws(TextureAsset textureAsset, int atlasPageSize, int currentX, int currentY, int page);
    }
}
