using System;
using System.Collections.Generic;
using System.IO;

namespace UOStudio.Client.Engine.IO
{
    internal class UopFileIndex : FileIndexBase
    {
        public const int UOP_MAGIC_NUMBER = 0x50594D;
        private readonly string _extension;
        private readonly bool _hasExtra;

        public UopFileIndex(string uopPath, int length, bool hasExtra, string extension)
            : base(uopPath, length)
        {
            _extension = extension;
            _hasExtra = hasExtra;
        }

        protected override FileIndexEntry[] ReadEntries()
        {
            var length = Length;
            var dataPath = DataPath;
            var entries = new FileIndexEntry[length];

            for (var i = 0; i < entries.Length; i++)
            {
                entries[i].Lookup = -1;
            }

            using var index = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileInfo = new FileInfo(dataPath);
            var uopPattern = Path.GetFileNameWithoutExtension(fileInfo.Name).ToLowerInvariant();

            using var binaryReader = new BinaryReader(index);
            binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);

            if (binaryReader.ReadInt32() != UOP_MAGIC_NUMBER)
            {
                throw new ArgumentException("Bad UOP file.");
            }

            binaryReader.ReadInt64(); // version + signature
            var nextBlock = binaryReader.ReadInt64();
            binaryReader.ReadInt32(); // block capacity
            var count = binaryReader.ReadInt32();

            var hashes = new Dictionary<ulong, int>();

            for (var i = 0; i < length; i++)
            {
                var entryName = $"build/{uopPattern}/{i:D8}{_extension}";
                var hash = CreateHash(entryName);

                if (!hashes.ContainsKey(hash))
                {
                    hashes.Add(hash, i);
                }
            }

            binaryReader.BaseStream.Seek(nextBlock, SeekOrigin.Begin);

            do
            {
                var filesCount = binaryReader.ReadInt32();
                nextBlock = binaryReader.ReadInt64();

                for (var i = 0; i < filesCount; i++)
                {
                    var offset = binaryReader.ReadInt64();
                    var headerLength = binaryReader.ReadInt32();
                    var compressedLength = binaryReader.ReadInt32();
                    var decompressedLength = binaryReader.ReadInt32();
                    var hash = binaryReader.ReadUInt64();
                    binaryReader.ReadUInt32(); // Adler32
                    var flag = binaryReader.ReadInt16();

                    var entryLength = flag == 1 ? compressedLength : decompressedLength;

                    if (offset == 0)
                    {
                        continue;
                    }

                    if (hashes.TryGetValue(hash, out var idx))
                    {
                        if (idx < 0 || idx > entries.Length)
                        {
                            throw new IndexOutOfRangeException("hashes dictionary and files collection have different count of entries!");
                        }

                        entries[idx].Lookup = (int)(offset + headerLength);
                        entries[idx].Length = entryLength;

                        if (_hasExtra)
                        {
                            var cursorPosition = binaryReader.BaseStream.Position;

                            binaryReader.BaseStream.Seek(offset + headerLength, SeekOrigin.Begin);

                            var extra = binaryReader.ReadBytes(8);

                            var extra1 = (ushort)((extra[3] << 24) | (extra[2] << 16) | (extra[1] << 8) | extra[0]);
                            var extra2 = (ushort)((extra[7] << 24) | (extra[6] << 16) | (extra[5] << 8) | extra[4]);

                            entries[idx].Lookup += 8;
                            entries[idx].Extra = extra1 << 16 | extra2;

                            binaryReader.BaseStream.Seek(cursorPosition, SeekOrigin.Begin);
                        }
                    }
                }
            }
            while (binaryReader.BaseStream.Seek(nextBlock, SeekOrigin.Begin) != 0);

            return entries;
        }

        public static ulong CreateHash(string s)
        {
            uint ecx, edx, ebx, esi, edi;

            var eax = ecx = edx = ebx = esi = edi = 0;
            ebx = edi = esi = (uint)s.Length + 0xDEADBEEF;

            var i = 0;

            for (i = 0; i + 12 < s.Length; i += 12)
            {
                edi = (uint)((s[i + 7] << 24) | (s[i + 6] << 16) | (s[i + 5] << 8) | s[i + 4]) + edi;
                esi = (uint)((s[i + 11] << 24) | (s[i + 10] << 16) | (s[i + 9] << 8) | s[i + 8]) + esi;
                edx = (uint)((s[i + 3] << 24) | (s[i + 2] << 16) | (s[i + 1] << 8) | s[i]) - esi;

                edx = (edx + ebx) ^ (esi >> 28) ^ (esi << 4);
                esi += edi;
                edi = (edi - edx) ^ (edx >> 26) ^ (edx << 6);
                edx += esi;
                esi = (esi - edi) ^ (edi >> 24) ^ (edi << 8);
                edi += edx;
                ebx = (edx - esi) ^ (esi >> 16) ^ (esi << 16);
                esi += edi;
                edi = (edi - ebx) ^ (ebx >> 13) ^ (ebx << 19);
                ebx += esi;
                esi = (esi - edi) ^ (edi >> 28) ^ (edi << 4);
                edi += ebx;
            }

            if (s.Length - i > 0)
            {
                switch (s.Length - i)
                {
                    case 12:
                        esi += (uint)s[i + 11] << 24;
                        goto case 11;
                    case 11:
                        esi += (uint)s[i + 10] << 16;
                        goto case 10;
                    case 10:
                        esi += (uint)s[i + 9] << 8;
                        goto case 9;
                    case 9:
                        esi += s[i + 8];
                        goto case 8;
                    case 8:
                        edi += (uint)s[i + 7] << 24;
                        goto case 7;
                    case 7:
                        edi += (uint)s[i + 6] << 16;
                        goto case 6;
                    case 6:
                        edi += (uint)s[i + 5] << 8;
                        goto case 5;
                    case 5:
                        edi += s[i + 4];
                        goto case 4;
                    case 4:
                        ebx += (uint)s[i + 3] << 24;
                        goto case 3;
                    case 3:
                        ebx += (uint)s[i + 2] << 16;
                        goto case 2;
                    case 2:
                        ebx += (uint)s[i + 1] << 8;
                        goto case 1;
                    case 1:
                        ebx += s[i];
                        break;
                }

                esi = (esi ^ edi) - ((edi >> 18) ^ (edi << 14));
                ecx = (esi ^ ebx) - ((esi >> 21) ^ (esi << 11));
                edi = (edi ^ ecx) - ((ecx >> 7) ^ (ecx << 25));
                esi = (esi ^ edi) - ((edi >> 16) ^ (edi << 16));
                edx = (esi ^ ecx) - ((esi >> 28) ^ (esi << 4));
                edi = (edi ^ edx) - ((edx >> 18) ^ (edx << 14));
                eax = (esi ^ edi) - ((edi >> 8) ^ (edi << 24));

                return ((ulong)edi << 32) | eax;
            }

            return ((ulong)esi << 32) | eax;
        }
    }
}
