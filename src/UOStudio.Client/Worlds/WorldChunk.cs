using Microsoft.Xna.Framework;
using UOStudio.Common.Network;

namespace UOStudio.Client.Worlds
{
    internal sealed class WorldChunk
    {
        public ChunkStaticTile[] Tiles { get; }

        public ChunkItemTile[] Items { get; }

        public Point Position { get; }

        public bool IsDirty { get; private set; }

        public WorldChunk(Point position)
        {
            Position = position;
            Tiles = new ChunkStaticTile[ChunkData.ChunkSize * ChunkData.ChunkSize];
            for (var i = 0; i < Tiles.Length; i++)
            {
                Tiles[i] = new ChunkStaticTile(0, 0, 0);
            }
            Items = new ChunkItemTile[ChunkData.ChunkSize * ChunkData.ChunkSize];
            for (var i = 0; i < Tiles.Length; i++)
            {
                Items[i] = new ChunkItemTile(0, 0, 0);
            }
            IsDirty = false;
        }

        public float GetZ(int x, int y)
        {
            var index = y * ChunkData.ChunkSize + x;
            if (index < 0 || index >= Tiles.Length)
            {
                return 0.0f;
            }

            var staticTile = Tiles[index];
            return staticTile.Z;
        }

        public ChunkStaticTile GetStaticTile(int x, int y)
        {
            return Tiles[y * ChunkData.ChunkSize + x];
        }

        public void UpdateWorldChunk(ref ChunkData chunkData)
        {
            for (var i = 0; i < chunkData.StaticTileData.Length; i++)
            {
                var chunkTile = Tiles[i];
                if (chunkTile.TileId != chunkData.StaticTileData[i].TileId ||
                    chunkTile.Z != chunkData.StaticTileData[i].Z ||
                    chunkTile.Hue != chunkData.StaticTileData[i].Hue)
                {
                    Tiles[i] = new ChunkStaticTile(chunkTile.TileId, chunkTile.Z, chunkTile.Hue);
                    IsDirty = true;
                }
            }

            for (var i = 0; i < chunkData.ItemTileData.Length; i++)
            {
                var chunkTile = Items[i];
                if (chunkTile.TileId != chunkData.ItemTileData[i].TileId ||
                    chunkTile.Z != chunkData.ItemTileData[i].Z ||
                    chunkTile.Hue != chunkData.ItemTileData[i].Hue)
                {
                    Items[i] = new ChunkItemTile(chunkTile.TileId, chunkTile.Z, chunkTile.Hue);
                    IsDirty = true;
                }
            }
        }
    }
}
