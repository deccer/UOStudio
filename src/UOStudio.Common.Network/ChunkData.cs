using System;
using System.Collections.Generic;
using System.Text;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;

namespace UOStudio.Common.Network
{
    public readonly struct ChunkData : INetSerializable
    {
        public const int ChunkSize = 16;

        public ChunkData(
            int worldId,
            Point position,
            ChunkStaticTileData[] staticTileData,
            ChunkItemTileData[] itemTileData)
        {
            WorldId = worldId;
            Position = position;
            StaticTileData = staticTileData;
            ItemTileData = itemTileData;
        }

        public int WorldId { get; }

        public Point Position { get; }

        public ChunkStaticTileData[] StaticTileData { get; }

        public ChunkItemTileData[] ItemTileData { get; }

        public void Deserialize(NetDataReader reader)
        {
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(WorldId);
            writer.Put(Position.X);
            writer.Put(Position.Y);
            writer.Put(ChunkData.ChunkSize);
            var chunkLength = ChunkData.ChunkSize * ChunkData.ChunkSize;
            for (int i = 0; i < chunkLength; i++)
            {
                var staticTileData = StaticTileData[i];
                writer.Put(staticTileData.TileId);
                writer.Put(staticTileData.Z);
                writer.Put(staticTileData.Hue);
            }

            for (int i = 0; i < chunkLength; i++)
            {
                var itemTileData = ItemTileData[i];
                writer.Put(itemTileData.TileId);
                writer.Put(itemTileData.Z);
                writer.Put(itemTileData.Hue);
            }
        }
    }
}
