using System.IO;

namespace UOStudio.Core.Ultima
{
    public class StaticItem : WorldItem
    {
        private ushort _hue;

        public StaticItem(WorldBlock owner, BinaryReader binaryReader, ushort x = 0, ushort y = 0)
            : base(owner)
        {
            if (binaryReader != null)
            {
                TileId = binaryReader.ReadUInt16();
                var ix = binaryReader.ReadByte();
                var iy = binaryReader.ReadByte();
                Z = binaryReader.ReadSByte();
                Hue = binaryReader.ReadUInt16();

                _x = (ushort)(x * 8 + ix);
                _y = (ushort)(y * 8 + iy);
            }
        }

        public ushort Hue
        {
            get => _hue;
            set
            {
                if (_hue == value)
                {
                    return;
                }

                _hue = value;
                DoChanged();
            }
        }

        public void UpdatePriorities(StaticTileData staticTileData, int solver)
        {
            if (!staticTileData.Flags.HasFlag(TileDataFlags.Background))
            {
                PriorityBonus++;
            }

            if (staticTileData.Height > 0)
            {
                PriorityBonus++;
            }
            PriorityBonus = 0;
            Priority = Z + PriorityBonus;
            PrioritySolver = solver;
        }

        public override int GetSize() => 7;

        public override void Write(BinaryWriter writer)
        {
            var ix = _x % 8;
            var iy = _y % 8;

            writer.Write(TileId);
            writer.Write(ix);
            writer.Write(iy);
            writer.Write(Z);
            writer.Write(Hue);
        }

        public override MulBlock Clone()
        {
            var staticItem = new StaticItem(null, null);
            staticItem.TileId = TileId;
            staticItem.X = X;
            staticItem.Y = Y;
            staticItem.Z = Z;
            staticItem._hue = _hue;
            return staticItem;
        }
    }
}
