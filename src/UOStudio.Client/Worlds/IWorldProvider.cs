using Microsoft.Xna.Framework;

namespace UOStudio.Client.Worlds
{
    interface IWorldProvider
    {
        WorldChunk GetChunk(int worldId, Point chunkPosition);

        World GetWorld(int worldId);
    }
}
