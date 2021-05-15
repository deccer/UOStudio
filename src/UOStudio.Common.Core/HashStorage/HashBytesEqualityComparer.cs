using System.Collections.Generic;

namespace UOStudio.Common.Core.HashStorage
{
    public sealed class HashBytesEqualityComparer : IEqualityComparer<byte[]>
    {
        /// <inheritdoc />
        public bool Equals(byte[] x, byte[] y)
        {
            if (x == null ^ y == null)
                return false;
            return ReferenceEquals(x, y) || ContentHash.Equals(x, y);
        }

        /// <inheritdoc />
        public int GetHashCode(byte[] hash)
        {
            // Implementation from http://stackoverflow.com/a/468084/24874

            unchecked
            {
                const int p = 0x1000193;

                var code = (int)0x811c9dc5;

                // ReSharper disable once LoopCanBeConvertedToQuery
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < hash.Length; i++)
                    code = (code ^ hash[i]) * p;

                code += code << 13;
                code ^= code >> 7;
                code += code << 3;
                code ^= code >> 17;
                code += code << 5;

                return code;
            }
        }
    }
}
