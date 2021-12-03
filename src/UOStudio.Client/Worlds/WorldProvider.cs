using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Serilog;

namespace UOStudio.Client.Worlds
{
    internal class WorldProvider : IWorldProvider
    {
        private readonly IDictionary<int, World> _worlds;
        private readonly ILogger _logger;

        public WorldProvider(ILogger logger)
        {
            _worlds = new Dictionary<int, World>(8);
            _logger = logger;
        }

        public World GetWorld(int worldId)
        {
            _logger.Debug("WorldProvider: trying to get world {@WorldId}", worldId);
            if (_worlds.TryGetValue(worldId, out var world))
            {
                return world;
            }

            _logger.Debug("WorldProvider: world not found, creating new one");
            world = new World();
            _worlds.Add(worldId, world);
            return world;
        }

        public WorldChunk GetChunk(int worldId, Point chunkPosition)
        {
            _logger.Debug("WorldProvider: trying to get chunk at position {@ChunkPosX},{@ChunkPosY}", chunkPosition.X, chunkPosition.Y);
            var world = GetWorld(worldId);
            return world?.GetChunk(chunkPosition);
        }
    }
}
