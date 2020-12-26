using Microsoft.Xna.Framework.Graphics;

namespace UOStudio.Client.Engine
{
    public class MapChunk
    {
        public const int ChunkSize = 8;

        private readonly Tile[] _tiles;

        public MapChunk(int x, int y)
        {
            X = x;
            Y = y;
            _tiles = new Tile[ChunkSize * ChunkSize];
        }

        public int X { get; }

        public int Y { get; }

        public Tile GetTile(int x, int y)
        {
            if (x < 0 || y < 0)
            {
                return null;
            }

            return _tiles[y * ChunkSize + x];
        }

        public void SetTile(int x, int y, Tile tile)
        {
            if (x < 0 || y < 0)
            {
                return;
            }

            _tiles[y * ChunkSize + x] = tile;
        }

        public void Draw(TileBatcher batcher, Texture2D texture)
        {
            foreach (var tile in _tiles)
            {
                tile.Draw(batcher, texture);
            }
        }
    }
}
