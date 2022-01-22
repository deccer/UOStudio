using System.Collections.Generic;

namespace UOStudio.TextureAtlasGenerator.Contracts
{
    public class Atlas
    {
        public int Width;

        public int Height;

        public int Depth;

        public IList<ItemTile> Items;

        public IList<LandTile> Lands;

        public IList<LandTile> LandTextures;
    }
}
