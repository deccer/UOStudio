using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace UOStudio.Client.Engine
{
    public class Map
    {
        public const sbyte MinimumZ = -125;

        private readonly IDictionary<int, MapChunk> _chunks;

        public Map()
        {
            _chunks = new Dictionary<int, MapChunk>(1024);
        }

        public Map(IReadOnlyList<sbyte> heightMap, int width, int height)
            : this()
        {
            for (ushort y = 0; y < height; ++y)
            {
                for (ushort x = 0; x < width; ++x)
                {
                    var index = y * width + x;
                    var heightValue = heightMap[index];
                    var chunk = GetChunk(x, y);
                    if (chunk == null)
                    {
                        chunk = new MapChunk(x, y);
                        var chunkX = x / MapChunk.ChunkSize;
                        var chunkY = y / MapChunk.ChunkSize;
                        _chunks.Add(chunkY * MapChunk.ChunkSize + chunkX, chunk);
                    }

                    var landTile = new LandTile(0, x, y, heightValue);
                    chunk.SetTile(x, y, landTile);

                    landTile.ApplyStretch(this, x, y, heightValue);
                    landTile.UpdateScreenPosition();
                }
            }
        }

        public MapChunk GetChunk(int x, int y)
        {
            if (x < 0 || y < 0)
            {
                return null;
            }

            var chunkX = x / MapChunk.ChunkSize;
            var chunkY = y / MapChunk.ChunkSize;
            return _chunks.TryGetValue(chunkY * MapChunk.ChunkSize + chunkX, out var chunk)
                ? chunk
                : null;
        }

        public Tile GetTile(int x, int y)
            => GetChunk(x, y)?.GetTile(x % MapChunk.ChunkSize, y % MapChunk.ChunkSize);

        public sbyte GetTileZ(int x, int y)
        {
            if (x < 0 || y < 0)
            {
                return MinimumZ;
            }

            var tile = GetTile(x, y);

            return tile switch
            {
                LandTile landTile => landTile.Z,
                StaticTile staticTile => staticTile.Z,
                _ => MinimumZ
            };
        }

        public void SetTile(int x, int y, Tile tile)
            => GetChunk(x, y)?.SetTile(x % MapChunk.ChunkSize, y % MapChunk.ChunkSize, tile);

        public void Draw(TileBatcher batcher, Texture2D texture)
        {
            foreach (var chunk in _chunks.Values)
            {
                chunk.Draw(batcher, texture);
            }
        }
    }
}
