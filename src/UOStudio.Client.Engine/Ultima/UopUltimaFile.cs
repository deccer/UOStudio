using System;
using System.Collections.Generic;
using Serilog;
using UOStudio.Core;

namespace UOStudio.Client.Engine.Ultima
{
    internal class UopUltimaFile : UltimaFile
    {
        private const uint UOP_MAGIC_NUMBER = 0x50594D;
        private readonly bool _hasExtra;
        private readonly Dictionary<ulong, UltimaFileIndex> _hashes = new Dictionary<ulong, UltimaFileIndex>();
        private readonly string _pattern;

        public UopUltimaFile(ILogger logger, string filePath, string pattern, bool hasExtra = false)
            : base(logger, filePath)
        {
            _pattern = pattern;
            _hasExtra = hasExtra;
            Load();
        }

        public int TotalEntriesCount { get; private set; }

        public bool TryGetUopData(ulong hash, out UltimaFileIndex data) => _hashes.TryGetValue(hash, out data);

        protected override void Load()
        {
            base.Load();

            Seek(0);

            if (ReadUInt() != UOP_MAGIC_NUMBER)
            {
                throw new ArgumentException("Bad uop file");
            }

            var version = ReadUInt();
            var format_timestamp = ReadUInt();
            var nextBlock = ReadInt64();
            var block_size = ReadUInt();
            var count = ReadInt32();

            Seek(nextBlock);
            var total = 0;
            var real_total = 0;

            do
            {
                var filesCount = ReadInt32();
                nextBlock = ReadInt64();
                total += filesCount;

                for (var i = 0; i < filesCount; i++)
                {
                    var offset = ReadInt64();
                    var headerLength = ReadInt32();
                    var compressedLength = ReadInt32();
                    var decompressedLength = ReadInt32();
                    var hash = ReadUInt64();
                    var dataHash = ReadUInt();
                    var flag = ReadInt16();
                    var length = flag == 1 ? compressedLength : decompressedLength;

                    if (offset == 0)
                    {
                        continue;
                    }

                    real_total++;
                    offset += headerLength;

                    if (_hasExtra)
                    {
                        var cursorPosition = Position;
                        Seek(offset);
                        var extra1 = (short)ReadInt32();
                        var extra2 = (short)ReadInt32();

                        _hashes.Add(
                            hash,
                            new UltimaFileIndex(
                                StartAddress,
                                (uint)Length,
                                offset + 8,
                                compressedLength - 8,
                                decompressedLength,
                                extra1,
                                extra2
                            )
                        );

                        Seek(cursorPosition);
                    }
                    else
                    {
                        _hashes.Add(hash, new UltimaFileIndex(StartAddress, (uint)Length, offset, compressedLength, decompressedLength));
                    }
                }

                Seek(nextBlock);
            }
            while (nextBlock != 0);

            TotalEntriesCount = real_total;
        }

        public void ClearHashes()
        {
            _hashes.Clear();
        }

        public override void Dispose()
        {
            ClearHashes();
            base.Dispose();
        }

        public override void FillEntries(ref UltimaFileIndex[] entries)
        {
            for (var i = 0; i < entries.Length; i++)
            {
                var file = string.Format(_pattern, i);
                var hash = CreateHash(file);

                if (_hashes.TryGetValue(hash, out var data))
                {
                    entries[i] = data;
                }
            }
        }

        public void FillEntries(ref UltimaFileIndex[] entries, bool clearHashes)
        {
            FillEntries(ref entries);

            if (clearHashes)
            {
                ClearHashes();
            }
        }

        public unsafe byte[] GetData(int compressedSize, int uncompressedSize)
        {
            var data = new byte[uncompressedSize];

            fixed (byte* destPtr = data)
            {
                ZLib.Decompress(PositionAddress, compressedSize, 0, (IntPtr)destPtr, uncompressedSize);
            }

            return data;
        }

        internal static ulong CreateHash(string s)
        {
            uint eax, ecx, edx, ebx, esi, edi;
            eax = ecx = edx = ebx = esi = edi = 0;
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
