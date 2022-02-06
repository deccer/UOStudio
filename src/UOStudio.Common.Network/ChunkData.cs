using LiteNetLib.Utils;
using UOStudio.Client.Engine.Mathematics;

namespace UOStudio.Common.Network
{
    public struct ChunkData : INetSerializable
    {
        public const int ChunkSize = 8;

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

        public Point Position { get; private set; }

        public ChunkStaticTileData[] StaticTileData { get; }

        public ChunkItemTileData[] ItemTileData { get; }

        public void Deserialize(NetDataReader reader)
        {
            var worldId = reader.GetInt();
            var chunkSize = reader.GetInt();
            var positionX = reader.GetInt();
            var positionY = reader.GetInt();
            Position = new Point(positionX, positionY);
            var chunkLength = ChunkData.ChunkSize * ChunkData.ChunkSize;
            for (var i = 0; i < chunkLength; i++)
            {
                StaticTileData[i] = new ChunkStaticTileData(
                    reader.GetUShort(),
                    reader.GetInt(),
                    reader.GetInt());
            }

            for (var i = 0; i < chunkLength; i++)
            {
                ItemTileData[i] = new ChunkItemTileData(
                    reader.GetUShort(),
                    reader.GetInt(),
                    reader.GetInt());
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(WorldId);
            writer.Put(ChunkData.ChunkSize);

            writer.Put(Position.X);
            writer.Put(Position.Y);
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
