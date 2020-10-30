using System;
using System.IO;

namespace UOStudio.Core.Ultima
{
    public abstract class IndexedMulProvider : MulProvider
    {
        private Stream _indexStream;
        private BinaryReader _indexReader;

        public uint EntryCount { get; }

        public IndexedMulProvider(
            Stream mulStream,
            Stream indexStream,
            bool readOnly = false)
            : base(mulStream, readOnly)
        {
            _indexStream = indexStream;
            _indexReader = new BinaryReader(indexStream);
            EntryCount = (uint)((int)indexStream.Length / 12);
        }

        public IndexedMulProvider(
            string mulFileName,
            string indexFileName,
            bool readOnly = false)
            : base(mulFileName, readOnly)
        {
            var fileMode = ReadOnly ? FileMode.Open : FileMode.CreateNew;
            var fileAccess = ReadOnly ? FileAccess.Read : FileAccess.ReadWrite;

            _indexStream = File.Open(indexFileName, fileMode, fileAccess);
            _indexReader = new BinaryReader(_indexStream);
            EntryCount = (uint)((int)_indexStream.Length / 12);
        }

        public bool Exists(int id)
        {
            _indexReader.BaseStream.Position = CalculateOffset(id);
            var genericIndex = new GenericIndex(_indexReader);

            return genericIndex.Lookup > -1 && genericIndex.Size > 0;
        }

        public override MulBlock GetBlock(int id)
        {
            GetBlockEx(id, out var result, out _);
            return result;
        }

        public void GetBlockEx(int id, out MulBlock block, out GenericIndex index)
        {
            _indexReader.BaseStream.Position = CalculateOffset(id);

            index = new GenericIndex(_indexReader);
            block = GetData(id, index);
            block.Changed += OnChanged;
            block.Finished += OnFinished;
        }

        public override void SetBlock(int id, MulBlock block)
        {
            if (ReadOnly)
            {
                return;
            }

            _indexReader.BaseStream.Position = CalculateOffset(id);
            var genericIndex = new GenericIndex(_indexReader);
            SetData(id, genericIndex, block);
            _indexReader.BaseStream.Position = CalculateOffset(id);
            genericIndex.Various = GetVarious(id, block, genericIndex.Various);
            var writer = new BinaryWriter(_indexStream);
            genericIndex.Write(writer);
        }

        protected override MulBlock GetData(int id, int offset) => throw new NotImplementedException();

        protected abstract MulBlock GetData(int id, GenericIndex index);

        protected void SetData(int id, GenericIndex index, MulBlock block)
        {
            if (ReadOnly)
            {
                return;
            }

            var size = block.GetSize();
            if (size == 0)
            {
                index.Lookup = -1;
                index.Various = -1;
            }
            else if (size > index.Size || index.Lookup < 0)
            {
                _dataStream.Seek(0, SeekOrigin.End);
                index.Lookup = (int)_dataStream.Position;
                block.Write(_writer);
            }
            else
            {
                _dataStream.Position = index.Lookup;
                block.Write(_writer);
            }
        }

        protected override int CalculateOffset(int id) => throw new NotImplementedException();

        protected virtual int GetVarious(int blockId, MulBlock block, int defaultValue) => defaultValue;
    }
}
