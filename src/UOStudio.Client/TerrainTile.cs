namespace UOStudio.Client
{
    internal readonly struct TerrainTile
    {
        public TerrainTile(ushort tileId, sbyte z)
        {
            TileId = tileId;
            Z = z;
        }

        public readonly ushort TileId;

        public readonly sbyte Z;
    }
}
