using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;

namespace UOStudio.Tools.TextureAtlasGenerator.Ultima
{
    public sealed class Textures
    {
        private static FileIndex _fileIndex = new FileIndex("Texidx.mul", "Texmaps.mul", 0x4000, 10);
        private static Bitmap[] _cache = new Bitmap[0x4000];
        private static bool[] _removed = new bool[0x4000];
        private static readonly Dictionary<int, bool> _patched = new Dictionary<int, bool>();

        private struct Checksums
        {
            public byte[] checksum;
            public int pos;
            public int length;
            public int extra;
        }

        /// <summary>
        /// ReReads texmaps
        /// </summary>
        public static void Reload()
        {
            _fileIndex = new FileIndex("Texidx.mul", "Texmaps.mul", 0x4000, 10);
            _cache = new Bitmap[0x4000];
            _removed = new bool[0x4000];
            _patched.Clear();
        }

        public static int GetIdxLength()
        {
            return (int)(_fileIndex.IdxLength / 12);
        }

        /// <summary>
        /// Removes Texture <see cref="_removed"/>
        /// </summary>
        /// <param name="index"></param>
        public static void Remove(int index)
        {
            _removed[index] = true;
        }

        /// <summary>
        /// Replaces Texture
        /// </summary>
        /// <param name="index"></param>
        /// <param name="bmp"></param>
        public static void Replace(int index, Bitmap bmp)
        {
            _cache[index] = bmp;
            _removed[index] = false;
            if (_patched.ContainsKey(index))
            {
                _patched.Remove(index);
            }
        }

        /// <summary>
        /// Tests if index is valid Texture
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool TestTexture(int index)
        {
            if (_removed[index])
            {
                return false;
            }

            if (_cache[index] != null)
            {
                return true;
            }

            var valid = _fileIndex.Valid(index, out var length, out var _, out var _);

            return valid && (length != 0);
        }

        /// <summary>
        /// Returns Bitmap of Texture
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Bitmap GetTexture(int index)
        {
            return GetTexture(index, out var _);
        }

        public static byte[] GetRawTexture(int index)
        {
            var stream = _fileIndex.Seek(index, out var length, out var extra, out _);
            if (stream == null)
            {
                return null;
            }

            if (length == 0)
            {
                return null;
            }

            var size = extra == 0 ? 64 : 128;
            var max = size * size * 2;

            var streamBuffer = new byte[max];

            stream.Read(streamBuffer, 0, max);

            return streamBuffer;
        }

        /// <summary>
        /// Returns Bitmap of Texture with verdata bool
        /// </summary>
        /// <param name="index"></param>
        /// <param name="patched"></param>
        /// <returns></returns>
        public static unsafe Bitmap GetTexture(int index, out bool patched)
        {
            patched = _patched.ContainsKey(index) && _patched[index];

            if (_removed[index])
            {
                return null;
            }

            if (_cache[index] != null)
            {
                return _cache[index];
            }

            var stream = _fileIndex.Seek(index, out var length, out var extra, out patched);
            if (stream == null)
            {
                return null;
            }

            if (length == 0)
            {
                return null;
            }

            if (patched)
            {
                _patched[index] = true;
            }

            var size = extra == 0 ? 64 : 128;

            var bmp = new Bitmap(size, size, PixelFormat.Format16bppArgb1555);
            var bd = bmp.LockBits(new Rectangle(0, 0, size, size), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);

            var line = (ushort*)bd.Scan0;
            var delta = bd.Stride >> 1;

            var max = size * size * 2;

            var streamBuffer = new byte[max];

            stream.Read(streamBuffer, 0, max);

            fixed (byte* data = streamBuffer)
            {
                var binData = (ushort*)data;
                for (var y = 0; y < size; ++y, line += delta)
                {
                    var cur = line;
                    var end = cur + size;

                    while (cur < end)
                    {
                        *cur++ = (ushort)(*binData++ ^ 0x8000);
                    }
                }
            }

            bmp.UnlockBits(bd);

            stream.Close();

            if (!Files.CacheData)
            {
                return _cache[index] = bmp;
            }

            return bmp;
        }

        public static unsafe void Save(string path)
        {
            var idx = Path.Combine(path, "texidx.mul");
            var mul = Path.Combine(path, "texmaps.mul");

            var checksumList = new List<Checksums>();

            var memIdx = new MemoryStream();
            var memMul = new MemoryStream();

            using var binIdx = new BinaryWriter(memIdx);
            using var binMul = new BinaryWriter(memMul);

            for (var index = 0; index < GetIdxLength(); ++index)
            {
                _cache[index] ??= GetTexture(index);

                var bmp = _cache[index];
                if (bmp == null || _removed[index])
                {
                    binIdx.Write(0); // lookup
                    binIdx.Write(0); // length
                    binIdx.Write(0); // extra
                }
                else
                {
                    using var sha = SHA256.Create();
                    using var ms = new MemoryStream();
                    bmp.Save(ms, ImageFormat.Bmp);
                    var newChecksum = sha.ComputeHash(ms.ToArray());

                    if (CompareSaveImages(checksumList, newChecksum, out var sum))
                    {
                        binIdx.Write(sum.pos);    // lookup
                        binIdx.Write(sum.length); // length
                        binIdx.Write(sum.extra);  // extra

                        continue;
                    }

                    var bd = bmp.LockBits(
                        new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                        PixelFormat.Format16bppArgb1555);
                    var line = (ushort*)bd.Scan0;
                    var delta = bd.Stride >> 1;

                    binIdx.Write((int)binMul.BaseStream.Position); // lookup
                    var length = (int)binMul.BaseStream.Position;

                    for (var y = 0; y < bmp.Height; ++y, line += delta)
                    {
                        var cur = line;
                        for (var x = 0; x < bmp.Width; ++x)
                        {
                            binMul.Write((ushort)(cur[x] ^ 0x8000));
                        }
                    }

                    var start = length;
                    length = (int)binMul.BaseStream.Position - length;
                    binIdx.Write(length);
                    var extra = GetExtraFlag(length);
                    binIdx.Write(extra);
                    bmp.UnlockBits(bd);

                    checksumList.Add(new Checksums
                    {
                        pos = start,
                        length = length,
                        checksum = newChecksum,
                        extra = extra
                    });
                }
            }

            using var fileIdx = new FileStream(idx, FileMode.Create, FileAccess.Write, FileShare.Write);
            using var fileMul = new FileStream(mul, FileMode.Create, FileAccess.Write, FileShare.Write);
            memIdx.WriteTo(fileIdx);
            memMul.WriteTo(fileMul);

            memIdx.Dispose();
            memIdx.Dispose();
        }

        private static int GetExtraFlag(int length)
        {
            // length of 0x8000 == width 128x128 else 64x64
            return length == 0x8000 ? 1 : 0;
        }

        private static bool CompareSaveImages(IReadOnlyList<Checksums> checksumList, IReadOnlyList<byte> newChecksum, out Checksums sum)
        {
            sum = new Checksums();
            for (var i = 0; i < checksumList.Count; ++i)
            {
                var cmp = checksumList[i].checksum;
                if ((cmp == null) || (newChecksum == null) || (cmp.Length != newChecksum.Count))
                {
                    return false;
                }

                var valid = true;

                for (var j = 0; j < cmp.Length; ++j)
                {
                    if (cmp[j] == newChecksum[j])
                    {
                        continue;
                    }

                    valid = false;
                    break;
                }

                if (!valid)
                {
                    continue;
                }

                sum = checksumList[i];
                return true;
            }

            return false;
        }
    }
}
