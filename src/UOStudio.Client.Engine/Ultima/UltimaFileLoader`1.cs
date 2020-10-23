using System.Collections.Generic;

namespace UOStudio.Client.Engine.Ultima
{
    public abstract class UltimaFileLoader<T> : UltimaFileLoader
        where T : UltimaTexture
    {
        private readonly LinkedList<uint> _usedTextures = new LinkedList<uint>();

        protected UltimaFileLoader(int max)
        {
            Resources = new T[max];
        }

        protected readonly T[] Resources;

        public abstract T GetTexture(uint id);

        protected void SaveId(uint id)
        {
            _usedTextures.AddLast(id);
        }

        public virtual void CleaUnusedResources(int count)
        {
            ClearUnusedResources(Resources, count);
        }

        public override void ClearResources()
        {
            var first = _usedTextures.First;

            while (first != null)
            {
                var next = first.Next;
                var idx = first.Value;

                if (idx < Resources.Length)
                {
                    ref var texture = ref Resources[idx];
                    texture?.Dispose();
                    texture = null;
                }

                _usedTextures.Remove(first);

                first = next;
            }
        }

        public void ClearUnusedResources(UltimaTexture[] resourceCache, int maxCount)
        {
            if (Time.Ticks <= 3000)
            {
                return;
            }

            var ticks = Time.Ticks - 3000;
            var count = 0;

            var first = _usedTextures.First;

            while (first != null)
            {
                var next = first.Next;
                var index = first.Value;

                if (index < resourceCache.Length && resourceCache[index] != null)
                {
                    if (resourceCache[index].Ticks < ticks)
                    {
                        if (count++ >= maxCount)
                        {
                            break;
                        }

                        resourceCache[index].Dispose();
                        resourceCache[index] = null;
                        _usedTextures.Remove(first);
                    }
                }

                first = next;
            }
        }


        public virtual bool TryGetEntryInfo(int entry, out long address, out long size, out long compressedSize)
        {
            address = size = compressedSize = 0;

            return false;
        }
    }
}
