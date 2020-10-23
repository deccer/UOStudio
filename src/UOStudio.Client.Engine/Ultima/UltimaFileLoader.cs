using System;
using System.Threading.Tasks;
using Serilog;

namespace UOStudio.Client.Engine.Ultima
{
    public abstract class UltimaFileLoader : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            ClearResources();
        }

        public UltimaFileIndex[] Entries;

        public abstract Task Load(ILogger logger);

        public virtual void ClearResources()
        {
        }

        public ref UltimaFileIndex GetEntry(int index)
        {
            if (index < 0 || Entries == null || index >= Entries.Length)
            {
                return ref UltimaFileIndex.Invalid;
            }

            ref var entry = ref Entries[index];

            if (entry.Offset < 0 || entry.Length <= 0)
            {
                return ref UltimaFileIndex.Invalid;
            }

            return ref entry;
        }
    }
}
