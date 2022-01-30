using UOStudio.Tools.TextureAtlasGenerator.Contracts;

namespace UOStudio.Tools.TextureAtlasGenerator.Abstractions
{
    public interface IUvwCalculator
    {
        Uvws CalculateUvws(TextureAsset textureAsset, int atlasPageSize, int currentX, int currentY, int page);

        TileType TileType { get; }
    }
}
