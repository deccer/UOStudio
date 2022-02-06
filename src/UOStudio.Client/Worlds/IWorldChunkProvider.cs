using UOStudio.Client.Engine.Mathematics;

namespace UOStudio.Client.Worlds
{
    public interface IWorldChunkProvider
    {
        WorldChunk GetChunk(int worldId, Point chunkPosition);
    }
}
