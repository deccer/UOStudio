using System.IO;

namespace UOStudio.Core.Ultima
{
    public class MapCell : WorldItem
    {
        public const int MapCellSize = 3;
        
        public MapCell(WorldBlock owner, BinaryReader reader, ushort x = 0, ushort y = 0)
            : base(owner)
        {
            X = x;
            Y = y;
            if (reader != null)
            {
                _tileId = reader.ReadUInt16();
                _z = reader.ReadSByte();
            }

            IsGhost = false;
        }

        public sbyte Altitude
        {
            get => IsGhost ? GhostZ : _z;
            set => SetZ(value);
        }

        public bool IsGhost { get; set; }

        public sbyte GhostZ { get; set; }

        public ushort GhostId { private get; set; }

        public override int GetSize() => MapCellSize;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(TileId);
            writer.Write(Z); // TODO bug? should be Altitude?
        }

        public override MulBlock Clone()
        {
            var mapCell = new MapCell(null, null);
            mapCell.X = _x;
            mapCell.Y = _y;
            mapCell.Z = _z; // TODO bug? should be GetZ?
            mapCell.TileId = _tileId;
            return mapCell;
        }

        private ushort GetTileId() => IsGhost
            ? GhostId
            : TileId;
    }
}
