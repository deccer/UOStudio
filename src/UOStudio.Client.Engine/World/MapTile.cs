namespace UOStudio.Client.Engine.World
{
    public readonly struct MapTile
    {
        public MapTile(short tileId, sbyte z)
        {
            TileId = tileId;
            Z = z;
        }

        public readonly short TileId;

        public readonly sbyte Z;
    }
}
