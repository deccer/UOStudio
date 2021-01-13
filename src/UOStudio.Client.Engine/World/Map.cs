using System.Collections.Generic;

namespace UOStudio.Client.Engine.World
{
    public class Map
    {
        private readonly IDictionary<(int X, int Y), MapChunk> _chunks;

        public Map()
        {
            _chunks = new Dictionary<(int X, int Y), MapChunk>();
        }

        public MapChunk GetChunk(int x, int y)
        {
            var chunkX = x / 8;
            var chunkY = y / 8;

            if (_chunks.TryGetValue((x, y), out var chunk))
            {
                return chunk;
            }

            return null;
        }
    }
}
