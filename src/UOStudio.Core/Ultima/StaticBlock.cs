using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UOStudio.Core.Ultima
{
    public class StaticBlock : WorldBlock
    {
        public IList<StaticItem> Items { get; }

        public StaticBlock(BinaryReader reader, GenericIndex index, ushort x = 0, ushort y = 0)
        {
            Items = new List<StaticItem>();
            if (reader != null && index.Lookup > 0 && index.Size > 0)
            {
                reader.BaseStream.Position = index.Lookup;
                var ms = new MemoryStream(index.Size);
                reader.BaseStream.CopyTo(ms, index.Size);
                ms.Position = 0;

                for (var i = 1; i < index.Size / 7; ++i)
                {
                    Items.Add(new StaticItem(this, reader, x, y));
                }
            }
        }

        public override int GetSize() => Items.Count * 7;

        public override void Write(BinaryWriter writer)
        {
            foreach (var item in Items)
            {
                item.Write(writer);
            }
        }

        public void ReverseWrite(BinaryWriter writer)
        {
            var itemsReverses = Items.Reverse();
            foreach (var item in itemsReverses)
            {
                item.Write(writer);
            }
        }

        public void Sort()
        {
            throw new NotImplementedException();
        }

        public override MulBlock Clone()
        {
            var clone = new StaticBlock(null, null, X, Y);
            foreach (var item in Items)
            {
                clone.Items.Add((StaticItem)item.Clone());
            }

            return clone;
        }
    }
}
