using System;

namespace UOStudio.Client.Worlds
{
    internal class ChunkStaticTile
    {
        public ChunkStaticTile(ushort tileId, int z, int hue)
        {
            TileId = tileId;
            Z = z;
            Hue = hue;
        }

        public ushort TileId { get; set; }

        public int Z { get; set; }

        public int Hue { get; set; }

        public override bool Equals(object other)
        {
            return Equals(other as ChunkStaticTile);
        }

        public virtual bool Equals(ChunkStaticTile other)
        {
            if (other == null) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return TileId == other.TileId && Z == other.Z && Hue == other.Hue;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TileId, Z, Hue);
        }

        public static bool operator ==(ChunkStaticTile item1, ChunkStaticTile item2)
        {
            if (object.ReferenceEquals(item1, item2)) { return true; }
            if ((object)item1 == null || (object)item2 == null) { return false; }
            return item1.TileId == item2.TileId && item1.Z == item2.Z && item1.Hue == item2.Hue;
        }

        public static bool operator !=(ChunkStaticTile item1, ChunkStaticTile item2)
        {
            return !(item1 == item2);
        }
    }
}
