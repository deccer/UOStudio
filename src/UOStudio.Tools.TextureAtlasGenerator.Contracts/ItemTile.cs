namespace UOStudio.TextureAtlasGenerator.Contracts
{
    public class ItemTile : Tile
    {
        public ItemTile(TextureAsset textureAsset, Uvws uvws)
            : base(textureAsset.TileId, uvws)
        {
            if (textureAsset.Bitmap == null)
            {
                Width = 0;
                Height = 0;
            }
            else
            {
                Width = textureAsset.Bitmap.Width;
                Height = textureAsset.Bitmap.Height;
            }
        }

        public int Height;

        public int Width;
    }
}