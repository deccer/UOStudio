using System;
using System.IO;

namespace UOStudio.Core.Ultima
{
    public abstract class MulProvider : IDisposable
    {
        protected Stream _dataStream;
        protected BinaryWriter _writer;
        private bool _ownsData;

        protected MulProvider(Stream stream, bool readOnly = false)
        {
            _dataStream = stream ?? throw new ArgumentNullException(nameof(stream));
            _writer = new BinaryWriter(_dataStream);
            ReadOnly = readOnly;
            _ownsData = false;
        }

        protected MulProvider(string fileName, bool readOnly = false)
        {
            var fileMode = readOnly ? FileMode.Open : FileMode.OpenOrCreate;
            var fileAccess = readOnly ? FileAccess.Read : FileAccess.ReadWrite;
            _dataStream = File.Open(fileName, fileMode, fileAccess);
            _writer = readOnly
                ? null
                : new BinaryWriter(_dataStream);
            ReadOnly = readOnly;
            _ownsData = true;
        }

        public event MulBlockChangedEventHandler Changed;

        public event MulBlockChangedEventHandler Finished;

        public MulBlock this[int blockId]
        {
            get => GetBlock(blockId);
            set => SetBlock(blockId, value);
        }

        public bool ReadOnly { get; }

        public void Dispose()
        {
            if (_ownsData)
            {
                _dataStream?.Dispose();
            }
        }

        public virtual MulBlock GetBlock(int id)
        {
            var block = GetData(id, CalculateOffset(id));
            block.Changed += OnChanged;
            block.Finished += OnFinished;
            return block;
        }

        public virtual void SetBlock(int id, MulBlock block)
        {
            if (ReadOnly)
            {
                return;
            }

            SetData(id, CalculateOffset(id), block);
        }

        protected abstract MulBlock GetData(int id, int offset);

        protected virtual void SetData(int id, int offset, MulBlock block)
        {
            if (ReadOnly)
            {
                return;
            }

            _dataStream.Position = offset;
            block.Write(_writer);
        }

        protected virtual void OnChanged(MulBlock mulBlock)
        {
            SetBlock(mulBlock.Id, mulBlock);
            var changed = Changed;
            changed?.Invoke(mulBlock);
        }

        protected virtual void OnFinished(MulBlock mulBlock)
        {
            var finished = Finished;
            finished?.Invoke(mulBlock);
        }

        protected abstract int CalculateOffset(int id);
    }
}
