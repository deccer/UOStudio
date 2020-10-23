using Serilog;

namespace UOStudio.Client.Engine.Ultima
{
    internal class MulUltimaFile : UltimaFile
    {
        private readonly int _count, _patch;
        private readonly MulIndexUltimaFile _indexFile;

        public MulUltimaFile(ILogger logger, string mulFileName, string indexFileName, int count, int patch = -1)
            : this(logger, mulFileName)
        {
            _indexFile = new MulIndexUltimaFile(logger, indexFileName);
            _count = count;
            _patch = patch;
        }

        public MulUltimaFile(ILogger logger, string mulFileName)
            : base(logger, mulFileName)
        {
            Load();
        }

        public UltimaFile IndexFile => _indexFile;


        public override void FillEntries(ref UltimaFileIndex[] entries)
        {
            var file = _indexFile ?? (UltimaFile)this;

            var count = (int)file.Length / 12;
            entries = new UltimaFileIndex[count];

            for (var i = 0; i < count; i++)
            {
                ref var entry = ref entries[i];
                entry.Address = StartAddress;    // .mul mmf address
                entry.FileSize = (uint)Length;   // .mul mmf length
                entry.Offset = file.ReadUInt();  // .idx offset
                entry.Length = file.ReadInt32(); // .idx length
                entry.DecompressedLength = 0;    // UNUSED HERE --> .UOP

                var size = file.ReadInt32();

                if (size > 0)
                {
                    entry.Width = (short)(size >> 16);
                    entry.Height = (short)(size & 0xFFFF);
                }
            }
        }

        public override void Dispose()
        {
            _indexFile?.Dispose();
            base.Dispose();
        }

        private class MulIndexUltimaFile : UltimaFile
        {
            public MulIndexUltimaFile(ILogger logger, string indexFileName)
                : base(logger, indexFileName)
            {
                Load();
            }

            public override void FillEntries(ref UltimaFileIndex[] entries)
            {
            }
        }
    }
}
