using Serilog;
using UOStudio.Client.Engine.Mathematics;

namespace UOStudio.Client.Worlds
{
    internal sealed class WorldChunkProvider : IWorldChunkProvider
    {
        private readonly ILogger _logger;
        private readonly IWorldProvider _worldProvider;

        public WorldChunkProvider(
            ILogger logger,
            IWorldProvider worldProvider)
        {
            _logger = logger;
            _worldProvider = worldProvider;
        }
        public WorldChunk GetChunk(int worldId, Point chunkPosition)
        {
            _logger.Debug("WorldProvider: trying to get chunk at position {@ChunkPosX},{@ChunkPosY}", chunkPosition.X, chunkPosition.Y);
            var world = _worldProvider.GetWorld(worldId);
            return world?.GetChunk(chunkPosition);
        }
    }
}
