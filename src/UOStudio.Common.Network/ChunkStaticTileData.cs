namespace UOStudio.Common.Network
{
    public readonly struct ChunkStaticTileData
    {
        public ChunkStaticTileData(
            ushort tileId,
            int z,
            int hue)
        {
            TileId = tileId;
            Z = z;
            Hue = hue;
        }

        public ushort TileId { get; }

        public int Z { get; }

        public int Hue { get; }
    }
}
