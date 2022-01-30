using UOStudio.Tools.TextureAtlasGenerator.Contracts;

namespace UOStudio.Tools.TextureAtlasGenerator.Abstractions
{
    public interface ITileContainer
    {
        void AddLandTile(LandTile tile);

        void AddLandTextureTile(LandTile landTile);

        void AddItemTile(ItemTile tile);

        void Save(string fileName, int atlasPageCount);
    }
}
