using System.Threading;

namespace UOStudio.Core.Ultima
{
    public abstract class WorldBlock : MulBlock
    {
        private int _refCount;

        public ushort X { get; set; }

        public ushort Y { get; set; }

        public int RefCount => _refCount;

        public bool Changed { get; set; }

        public void AddRef() => Interlocked.Increment(ref _refCount);

        public void RemoveRef() => Interlocked.Decrement(ref _refCount);
    }
}
