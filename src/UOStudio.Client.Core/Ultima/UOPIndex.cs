using System;
using System.Collections.Generic;
using System.IO;

namespace UOStudio.Client.Engine.Ultima
{
    public class UOPIndex
    {
        private readonly UOPEntry[] m_Entries;
        private readonly int m_Length;

        private readonly BinaryReader m_Reader;

        public UOPIndex(Stream stream)
        {
            m_Reader = new BinaryReader(stream);
            m_Length = (int)stream.Length;

            if (m_Reader.ReadInt32() != 0x50594D)
            {
                throw new ArgumentException("Invalid UOP file.");
            }

            Version = m_Reader.ReadInt32();
            m_Reader.ReadInt32();
            var nextTable = m_Reader.ReadInt32();

            var entries = new List<UOPEntry>();

            do
            {
                stream.Seek(nextTable, SeekOrigin.Begin);
                var count = m_Reader.ReadInt32();
                nextTable = m_Reader.ReadInt32();
                m_Reader.ReadInt32();

                for (var i = 0; i < count; ++i)
                {
                    var offset = m_Reader.ReadInt32();

                    if (offset == 0)
                    {
                        stream.Seek(30, SeekOrigin.Current);
                        continue;
                    }

                    m_Reader.ReadInt64();
                    var length = m_Reader.ReadInt32();

                    entries.Add(new UOPEntry(offset, length));

                    stream.Seek(18, SeekOrigin.Current);
                }
            }
            while (nextTable != 0 && nextTable < m_Length);

            entries.Sort(OffsetComparer.Instance);

            for (var i = 0; i < entries.Count; ++i)
            {
                stream.Seek(entries[i].Offset + 2, SeekOrigin.Begin);

                int dataOffset = m_Reader.ReadInt16();
                entries[i].Offset += 4 + dataOffset;

                stream.Seek(dataOffset, SeekOrigin.Current);
                entries[i].Order = m_Reader.ReadInt32();
            }

            entries.Sort();
            m_Entries = entries.ToArray();
        }

        public int Version { get; }

        public int Lookup(int offset)
        {
            var total = 0;

            for (var i = 0; i < m_Entries.Length; ++i)
            {
                var newTotal = total + m_Entries[i].Length;

                if (offset < newTotal)
                {
                    return m_Entries[i].Offset + (offset - total);
                }

                total = newTotal;
            }

            return m_Length;
        }

        public void Close()
        {
            m_Reader.Close();
        }

        private class UOPEntry : IComparable<UOPEntry>
        {
            public readonly int Length;
            public int Offset;
            public int Order;

            public UOPEntry(int offset, int length)
            {
                Offset = offset;
                Length = length;
                Order = 0;
            }

            public int CompareTo(UOPEntry other) => Order.CompareTo(other.Order);
        }

        private class OffsetComparer : IComparer<UOPEntry>
        {
            public static readonly IComparer<UOPEntry> Instance = new OffsetComparer();

            public int Compare(UOPEntry x, UOPEntry y) =>
                x == null ? y == null ? 0 : 1 :
                y == null ? -1 : x.Offset.CompareTo(y.Offset);
        }
    }
}
