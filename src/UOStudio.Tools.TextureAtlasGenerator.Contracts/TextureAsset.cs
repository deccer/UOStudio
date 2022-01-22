using System.Drawing;

namespace UOStudio.TextureAtlasGenerator.Contracts
{
    public struct TextureAsset
    {
        public int TileId;

        public readonly TileType TileType;

        public readonly string ArtHash;

        public readonly Bitmap Bitmap;

        public TextureAsset(
            int tileId,
            TileType tileType,
            string artHash,
            Bitmap bitmap)
        {
            TileId = tileId;
            TileType = tileType;
            ArtHash = artHash;
            Bitmap = bitmap;
        }
    }
}