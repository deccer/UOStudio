using System.Collections.Generic;
using System.IO;

namespace UOStudio.Client.Engine.IO
{
    internal class MulFileIndex : FileIndexBase
    {
        private readonly string _indexPath;
        private readonly FileInfo _fileInfo;

        public MulFileIndex(string idxFile, string mulFile)
            : base(mulFile)
        {
            _indexPath = idxFile;
            _fileInfo = new FileInfo(_indexPath);
        }

        public override bool FilesExist => File.Exists(_indexPath) && base.FilesExist;

        protected override FileIndexEntry[] ReadEntries()
        {
            var entries = new List<FileIndexEntry>();

            var length = (int)(_fileInfo.Length / 3 / 4);

            using var index = new FileStream(_indexPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var binaryReader = new BinaryReader(index);

            var count = (int)(index.Length / 12);

            for (var i = 0; i < count && i < length; ++i)
            {
                var entry = new FileIndexEntry
                {
                    Lookup = binaryReader.ReadInt32(),
                    Length = binaryReader.ReadInt32(),
                    Extra = binaryReader.ReadInt32()
                };

                entries.Add(entry);
            }

            for (var i = count; i < length; ++i)
            {
                var entry = new FileIndexEntry
                {
                    Lookup = -1,
                    Length = -1,
                    Extra = -1
                };

                entries.Add(entry);
            }

            return entries.ToArray();
        }
    }
}
