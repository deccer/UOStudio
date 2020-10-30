using System.IO;

namespace UOStudio.Core.Ultima
{
    public class MapBlock : WorldBlock
    {
        public const int MapBlockSize = 4 + 64 * MapCell.MapCellSize;

        protected readonly int _header;

        public MapBlock(BinaryReader reader, ushort x = 0, ushort y = 0)
        {
            X = x;
            Y = y;
            Cells = new MapCell[64];
            if (reader != null)
            {
                var ms = new MemoryStream(196);
                reader.BaseStream.CopyTo(ms, 196);
                ms.Position = 0;

                var bufferReader = new BinaryReader(ms);
                _header = bufferReader.ReadInt32();

                for (var iX = 0; iX < 8; ++iX)
                {
                    for (var iY = 0; iY < 8; ++iY)
                    {
                        Cells[iY * 8 + iX] = new MapCell(this, bufferReader, (ushort)(x * 8 + iX), (ushort)(y * 8 + iY));
                    }
                }
            }

            Changed = false;
        }

        public MapCell[] Cells { get; }

        public override int GetSize() => MapBlockSize;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(_header);
            for (var i = 0; i < 64; ++i)
            {
                Cells[i].Write(writer);
            }
        }

        public override MulBlock Clone()
        {
            var mapBlock = new MapBlock(null);
            mapBlock.X = X;
            mapBlock.Y = Y;
            for (var i = 0; i < 64; ++i)
            {
                mapBlock.Cells[i] = (MapCell)Cells[i].Clone();
            }

            return mapBlock;
        }
    }
}
