using System;
using System.Collections.Generic;
using System.IO;
using UOStudio.Core.Ultima;

namespace UOStudio.Client.Core
{
    public class SeparatedStaticBlock : StaticBlock
    {
        public SeparatedStaticBlock(BinaryReader reader, GenericIndex index, ushort x = 0, ushort y = 0)
            : base(reader, index, x, y)
        {
            Cells = new IList<StaticItem>[64];
            for (var i = 0; i < 64; ++i)
            {
                Cells[i] = new List<StaticItem>();
            }

            if (reader != null && index.Lookup > 0 && index.Size > 0)
            {
                reader.BaseStream.Position = index.Lookup;
                using var ms = new MemoryStream(index.Size);
                reader.BaseStream.CopyTo(ms, index.Size);
                ms.Position = 0;
                for (var i = 1; i <= index.Size / 7; ++i)
                {
                    var staticItemReader = new BinaryReader(ms);
                    var staticItem = new StaticItem(this, staticItemReader, x, y);
                    Cells[staticItem.Y % 8 * 8 + staticItem.X % 8].Add(staticItem);
                }
            }
        }

        public IList<StaticItem>[] Cells { get; }

        public override MulBlock Clone() => throw new NotImplementedException();

        public override int GetSize()
        {
            RebuildList();
            return base.GetSize();
        }

        public void RebuildList()
        {
            Items.Clear();
            var solver = 0;
            for (var i = 0; i < 64; ++i)
            {
                if (Cells[i] != null)
                {
                    for (var j = 0; j < Cells[i].Count; ++j)
                    {
                        Items.Add(Cells[i][j]);
                        //Cells[i][j].UpdatePriorities(ResMan.Tiledata.StaticTiles[TStaticItem(Cells[i].Items[j]).TileID],solver);
                        //TODO ^ wants global access to ResMan
                    }
                }
            }
        }
    }
}
