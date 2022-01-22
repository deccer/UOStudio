using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace UOStudio.TextureAtlasGenerator.Ultima
{
    public sealed class Gumps
    {
        private static FileIndex _fileIndex = new FileIndex(
            "Gumpidx.mul", "Gumpart.mul", "gumpartLegacyMUL.uop", 0xFFFF, 12, ".tga", -1, true);

        private static Bitmap[] _cache;
        private static bool[] _removed;
        private static readonly Dictionary<int, bool> _patched = new Dictionary<int, bool>();

        private static byte[] _pixelBuffer;
        private static byte[] _streamBuffer;
        private static byte[] _colorTable;

        static Gumps()
        {
            if (_fileIndex != null)
            {
                _cache = new Bitmap[_fileIndex.Index.Length];
                _removed = new bool[_fileIndex.Index.Length];
            }
            else
            {
                _cache = new Bitmap[0xFFFF];
                _removed = new bool[0xFFFF];
            }
        }

        /// <summary>
        /// ReReads gumpart
        /// </summary>
        public static void Reload()
        {
            try
            {
                _fileIndex = new FileIndex("Gumpidx.mul", "Gumpart.mul", "gumpartLegacyMUL.uop", 0xFFFF, 12, ".tga", -1, true);
                _cache = new Bitmap[_fileIndex.Index.Length];
                _removed = new bool[_fileIndex.Index.Length];
            }
            catch
            {
                _fileIndex = null;
                _cache = new Bitmap[0xFFFF];
                _removed = new bool[0xFFFF];
            }

            //_pixelBuffer = null;
            _streamBuffer = null;
            //_colorTable = null;
            _patched.Clear();
        }

        public static int GetCount()
        {
            return _cache.Length;
        }

        /// <summary>
        /// Replaces Gump <see cref="_cache"/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="bmp"></param>
        public static void ReplaceGump(int index, Bitmap bmp)
        {
            _cache[index] = bmp;
            _removed[index] = false;
            if (_patched.ContainsKey(index))
            {
                _patched.Remove(index);
            }
        }

        /// <summary>
        /// Removes Gumpindex <see cref="_removed"/>
        /// </summary>
        /// <param name="index"></param>
        public static void RemoveGump(int index)
        {
            _removed[index] = true;
        }

        /// <summary>
        /// Tests if index is defined
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool IsValidIndex(int index)
        {
            if (_fileIndex == null)
            {
                return false;
            }

            if (index > _cache.Length - 1)
            {
                return false;
            }

            if (_removed[index])
            {
                return false;
            }

            if (_cache[index] != null)
            {
                return true;
            }

            if (!_fileIndex.Valid(index, out var _, out var extra, out var _))
            {
                return false;
            }

            if (extra == -1)
            {
                return false;
            }

            var width = (extra >> 16) & 0xFFFF;
            var height = extra & 0xFFFF;

            return width > 0 && height > 0;
        }

        public static byte[] GetRawGump(int index, out int width, out int height)
        {
            width = -1;
            height = -1;

            var stream = _fileIndex.Seek(index, out var length, out var extra, out var _);

            if (stream == null)
            {
                return null;
            }

            if (extra == -1)
            {
                return null;
            }

            width = (extra >> 16) & 0xFFFF;
            height = extra & 0xFFFF;
            if (width <= 0 || height <= 0)
            {
                return null;
            }

            var buffer = new byte[length];
            stream.Read(buffer, 0, length);
            stream.Close();

            return buffer;
        }

        /// <summary>
        /// Returns Bitmap of index and applies Hue
        /// </summary>
        /// <param name="index"></param>
        /// <param name="hue"></param>
        /// <param name="onlyHueGrayPixels"></param>
        /// <param name="patched"></param>
        /// <returns></returns>
        public static unsafe Bitmap GetGump(int index, Hue hue, bool onlyHueGrayPixels, out bool patched)
        {
            var stream = _fileIndex.Seek(index, out var length, out var extra, out patched);

            if (stream == null)
            {
                return null;
            }

            if (extra == -1)
            {
                stream.Close();
                return null;
            }

            var width = (extra >> 16) & 0xFFFF;
            var height = extra & 0xFFFF;

            if (width <= 0 || height <= 0)
            {
                stream.Close();
                return null;
            }

            var bytesPerLine = width << 1;
            var bytesPerStride = (bytesPerLine + 3) & ~3;
            var bytesForImage = height * bytesPerStride;

            var pixelsPerStride = (width + 1) & ~1;
            var pixelsPerStrideDelta = pixelsPerStride - width;

            var pixelBuffer = _pixelBuffer;

            if (pixelBuffer == null || pixelBuffer.Length < bytesForImage)
            {
                _pixelBuffer = pixelBuffer = new byte[(bytesForImage + 2047) & ~2047];
            }

            var streamBuffer = _streamBuffer;

            if (streamBuffer == null || streamBuffer.Length < length)
            {
                _streamBuffer = streamBuffer = new byte[(length + 2047) & ~2047];
            }

            var colorTable = _colorTable;

            if (colorTable == null)
            {
                _colorTable = colorTable = new byte[128];
            }

            stream.Read(streamBuffer, 0, length);

            fixed (short* psHueColors = hue.Colors)
            {
                fixed (byte* pbStream = streamBuffer)
                {
                    fixed (byte* pbPixels = pixelBuffer)
                    {
                        fixed (byte* pbColorTable = colorTable)
                        {
                            var pHueColors = (ushort*)psHueColors;
                            var pHueColorsEnd = pHueColors + 32;

                            var pColorTable = (ushort*)pbColorTable;

                            var pColorTableOpaque = pColorTable;

                            while (pHueColors < pHueColorsEnd)
                            {
                                *pColorTableOpaque++ = *pHueColors++;
                            }

                            var pPixelDataStart = (ushort*)pbPixels;

                            var pLookup = (int*)pbStream;
                            var pLookupEnd = pLookup + height;
                            var pPixelRleStart = pLookup;
                            int* pPixelRle;

                            var pPixel = pPixelDataStart;
                            ushort* pRleEnd;
                            var pPixelEnd = pPixel + width;

                            ushort color, count;

                            if (onlyHueGrayPixels)
                            {
                                while (pLookup < pLookupEnd)
                                {
                                    pPixelRle = pPixelRleStart + *pLookup++;
                                    pRleEnd = pPixel;

                                    while (pPixel < pPixelEnd)
                                    {
                                        color = *(ushort*)pPixelRle;
                                        count = *(1 + (ushort*)pPixelRle);
                                        ++pPixelRle;

                                        pRleEnd += count;

                                        if (color != 0 && (color & 0x1F) == ((color >> 5) & 0x1F) && (color & 0x1F) == ((color >> 10) & 0x1F))
                                        {
                                            color = pColorTable[color >> 10];
                                        }
                                        else if (color != 0)
                                        {
                                            color ^= 0x8000;
                                        }

                                        while (pPixel < pRleEnd)
                                        {
                                            *pPixel++ = color;
                                        }
                                    }

                                    pPixel += pixelsPerStrideDelta;
                                    pPixelEnd += pixelsPerStride;
                                }
                            }
                            else
                            {
                                while (pLookup < pLookupEnd)
                                {
                                    pPixelRle = pPixelRleStart + *pLookup++;
                                    pRleEnd = pPixel;

                                    while (pPixel < pPixelEnd)
                                    {
                                        color = *(ushort*)pPixelRle;
                                        count = *(1 + (ushort*)pPixelRle);
                                        ++pPixelRle;

                                        pRleEnd += count;

                                        if (color != 0)
                                        {
                                            color = pColorTable[color >> 10];
                                        }

                                        while (pPixel < pRleEnd)
                                        {
                                            *pPixel++ = color;
                                        }
                                    }

                                    pPixel += pixelsPerStrideDelta;
                                    pPixelEnd += pixelsPerStride;
                                }
                            }

                            stream.Close();

                            return new Bitmap(width, height, bytesPerStride, PixelFormat.Format16bppArgb1555, (IntPtr)pPixelDataStart);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns Bitmap of index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Bitmap GetGump(int index)
        {
            return GetGump(index, out var _);
        }

        /// <summary>
        /// Returns Bitmap of index and if verdata patched
        /// </summary>
        /// <param name="index"></param>
        /// <param name="patched"></param>
        /// <returns></returns>
        public static unsafe Bitmap GetGump(int index, out bool patched)
        {
            patched = _patched.ContainsKey(index) && _patched[index];

            if (index > _cache.Length - 1)
            {
                return null;
            }

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

            if (extra == -1)
            {
                stream.Close();
                return null;
            }
            if (patched)
            {
                _patched[index] = true;
            }

            var width = (extra >> 16) & 0xFFFF;
            var height = extra & 0xFFFF;

            if (width <= 0 || height <= 0)
            {
                return null;
            }

            var bmp = new Bitmap(width, height, PixelFormat.Format16bppArgb1555);
            var bd = bmp.LockBits(
                new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);

            if (_streamBuffer == null || _streamBuffer.Length < length)
            {
                _streamBuffer = new byte[length];
            }

            stream.Read(_streamBuffer, 0, length);

            fixed (byte* data = _streamBuffer)
            {
                var lookup = (int*)data;
                var dat = (ushort*)data;

                var line = (ushort*)bd.Scan0;
                var delta = bd.Stride >> 1;
                for (var y = 0; y < height; ++y, line += delta)
                {
                    var count = (*lookup++ * 2);

                    var cur = line;
                    var end = line + bd.Width;

                    while (cur < end)
                    {
                        var color = dat[count++];
                        var next = cur + dat[count++];

                        if (color == 0)
                        {
                            cur = next;
                        }
                        else
                        {
                            color ^= 0x8000;
                            while (cur < next)
                            {
                                *cur++ = color;
                            }
                        }
                    }
                }
            }

            bmp.UnlockBits(bd);
            if (Files.CacheData)
            {
                return _cache[index] = bmp;
            }

            return bmp;
        }

        public static unsafe void Save(string path)
        {
            var idx = Path.Combine(path, "Gumpidx.mul");
            var mul = Path.Combine(path, "Gumpart.mul");

            using (var fsidx = new FileStream(idx, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (var fsmul = new FileStream(mul, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (var binidx = new BinaryWriter(fsidx))
            using (var binmul = new BinaryWriter(fsmul))
            {
                for (var index = 0; index < _cache.Length; index++)
                {
                    if (_cache[index] == null)
                    {
                        _cache[index] = GetGump(index);
                    }

                    var bmp = _cache[index];
                    if ((bmp == null) || (_removed[index]))
                    {
                        binidx.Write(-1); // lookup
                        binidx.Write(-1); // length
                        binidx.Write(-1); // extra
                    }
                    else
                    {
                        var bd = bmp.LockBits(
                            new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                            PixelFormat.Format16bppArgb1555);
                        var line = (ushort*)bd.Scan0;
                        var delta = bd.Stride >> 1;

                        binidx.Write((int)fsmul.Position); //lookup
                        var length = (int)fsmul.Position;
                        const int fill = 0;
                        for (var i = 0; i < bmp.Height; ++i)
                        {
                            binmul.Write(fill);
                        }

                        for (var y = 0; y < bmp.Height; ++y, line += delta)
                        {
                            var cur = line;

                            var x = 0;
                            var current = (int)fsmul.Position;
                            fsmul.Seek(length + (y * 4), SeekOrigin.Begin);
                            var offset = (current - length) / 4;
                            binmul.Write(offset);
                            fsmul.Seek(length + (offset * 4), SeekOrigin.Begin);

                            while (x < bd.Width)
                            {
                                var run = 1;
                                var c = cur[x];
                                while ((x + run) < bd.Width)
                                {
                                    if (c != cur[x + run])
                                    {
                                        break;
                                    }

                                    ++run;
                                }

                                if (c == 0)
                                {
                                    binmul.Write(c);
                                }
                                else
                                {
                                    binmul.Write((ushort)(c ^ 0x8000));
                                }

                                binmul.Write((short)run);
                                x += run;
                            }
                        }

                        length = (int)fsmul.Position - length;
                        binidx.Write(length);
                        binidx.Write((bmp.Width << 16) + bmp.Height);
                        bmp.UnlockBits(bd);
                    }
                }
            }
        }
    }
}