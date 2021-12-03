using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace UOStudio.Client.Worlds
{
    internal class World
    {
        private readonly IDictionary<Point, WorldChunk> _chunks;

        public World()
        {
            _chunks = new Dictionary<Point, WorldChunk>(256);
        }

        public WorldChunk GetChunk(Point chunkPosition)
        {
            if (_chunks.TryGetValue(chunkPosition, out var chunk))
            {
                return chunk;
            }

            chunk = new WorldChunk(chunkPosition);
            _chunks.Add(chunkPosition, chunk);
            return chunk;
        }

        public IReadOnlyCollection<WorldChunk> GetVisibleWorldChunks(Camera camera)
        {
            var visibleChunks = new List<WorldChunk>(16);
            return null;
        }
    }
}
