using UOStudio.TextureAtlasGenerator.Abstractions;
using UOStudio.TextureAtlasGenerator.Contracts;

namespace UOStudio.TextureAtlasGenerator
{
    internal sealed class LandUvwCalculator : IUvwCalculator
    {
        public Uvws CalculateUvws(TextureAsset textureAsset, int atlasPageSize, int currentX, int currentY, int page)
        {
            var atlasX = currentX / (float)atlasPageSize;
            var atlasY = currentY / (float)atlasPageSize;
            var atlasTileWidth = textureAsset.Bitmap.Width / (float)atlasPageSize;
            var atlasTileHeight = textureAsset.Bitmap.Height / (float)atlasPageSize;

            var uvws = new Uvws
            {
                V1 = new Vertex
                {
                    U = atlasX + atlasTileWidth * 0.5f,
                    V = atlasY,
                    W = page
                },
                V2 = new Vertex
                {
                    U = atlasX + atlasTileWidth,
                    V = atlasY + atlasTileHeight * 0.5f,
                    W = page
                },
                V3 = new Vertex
                {
                    U = atlasX,
                    V = atlasY + atlasTileHeight * 0.5f,
                    W = page
                },
                V4 = new Vertex
                {
                    U = atlasX + atlasTileWidth * 0.5f,
                    V = atlasY + atlasTileHeight,
                    W = page
                }
            };
            return uvws;
        }

        public TileType TileType => TileType.Land;
    }
}
