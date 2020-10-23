using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serilog;

namespace UOStudio.Client.Engine.Ultima
{
    public sealed class UltimaArtLoader : UltimaFileLoader<UltimaArtTexture>
    {
        private UltimaFile _file;
        private readonly ushort _graphicMask;
        private readonly UltimaTexture[] _landResources;
        private readonly LinkedList<uint> _usedLandTextureIds = new LinkedList<uint>();
        private readonly GraphicsDevice _graphicsDevice;
        private readonly string _ultimaOnlineBasePath;
        private readonly bool _isUop;

        public UltimaArtLoader(GraphicsDevice graphicsDevice, string ultimaOnlineBasePath, int staticCount, int landCount, bool isUop)
            : base(staticCount)
        {
            _graphicsDevice = graphicsDevice;
            _ultimaOnlineBasePath = ultimaOnlineBasePath;
            _isUop = isUop;
            _graphicMask = isUop ? (ushort)0xFFFF : (ushort)0x3FFF;
            _landResources = new UltimaTexture[landCount];
        }

        private string GetFilePath(string fileName) => Path.Combine(_ultimaOnlineBasePath, fileName);

        public override Task Load(ILogger logger)
        {
            return Task.Run
            (
                () =>
                {
                    string filePath = GetFilePath("artLegacyMUL.uop");

                    if (_isUop && File.Exists(filePath))
                    {
                        _file = new UopUltimaFile(logger, filePath, "build/artlegacymul/{0:D8}.tga");
                        Entries = new UltimaFileIndex[UltimaConstants.MaxStaticDataIndexCount];
                    }
                    else
                    {
                        filePath = GetFilePath("art.mul");
                        string indexMulFileName = GetFilePath("artidx.mul");

                        if (File.Exists(filePath) && File.Exists(indexMulFileName))
                        {
                            _file = new MulUltimaFile(logger, filePath, indexMulFileName, UltimaConstants.MaxStaticDataIndexCount);
                        }
                    }

                    _file.FillEntries(ref Entries);
                }
            );
        }

        public override UltimaArtTexture GetTexture(uint g)
        {
            if (g >= Resources.Length)
            {
                return null;
            }

            ref var texture = ref Resources[g];

            if (texture == null || texture.IsDisposed)
            {
                ReadStaticArt(ref texture, (ushort)g);

                if (texture != null)
                {
                    SaveId(g);
                }
            }
            else
            {
                texture.Ticks = Time.Ticks;
            }

            return texture;
        }

        public UltimaTexture GetLandTexture(uint g)
        {
            if (g >= _landResources.Length)
            {
                return null;
            }

            ref var texture = ref _landResources[g];

            if (texture?.IsDisposed != false)
            {
                ReadLandArt(ref texture, (ushort)g);

                if (texture != null)
                {
                    _usedLandTextureIds.AddLast(g);
                }
            }
            else
            {
                texture.Ticks = Time.Ticks;
            }

            return texture;
        }

        public override bool TryGetEntryInfo(int entry, out long address, out long size, out long compressedSize)
        {
            entry += 0x4000;

            if (entry < _file.Length && entry >= 0)
            {
                ref var fileIndex = ref GetEntry(entry);

                address = _file.StartAddress.ToInt64() + fileIndex.Offset;
                size = fileIndex.DecompressedLength == 0 ? fileIndex.Length : fileIndex.DecompressedLength;
                compressedSize = fileIndex.Length;

                return true;
            }

            return base.TryGetEntryInfo(entry, out address, out size, out compressedSize);
        }

        public override void ClearResources()
        {
            base.ClearResources();

            var first = _usedLandTextureIds.First;

            while (first != null)
            {
                var next = first.Next;
                var idx = first.Value;

                if (idx < _landResources.Length)
                {
                    ref var texture = ref _landResources[idx];
                    texture?.Dispose();
                    texture = null;
                }

                _usedLandTextureIds.Remove(first);

                first = next;
            }
        }

        public override void CleaUnusedResources(int count)
        {
            base.CleaUnusedResources(count);
            ClearUnusedResources(_landResources, count);
        }

        public unsafe uint[] ReadStaticArt(ushort graphic, out short width, out short height, out Rectangle bounds)
        {
            ref var entry = ref GetEntry(graphic + 0x4000);

            bounds = Rectangle.Empty;

            if (entry.Length == 0)
            {
                width = 0;
                height = 0;

                return null;
            }

            _file.Seek(entry.Offset);
            _file.Skip(4);
            width = _file.ReadInt16();
            height = _file.ReadInt16();

            if (width == 0 || height == 0)
            {
                return null;
            }

            var pixels = new uint[width * height];
            var ptr = (ushort*)_file.PositionAddress;
            var lineOffsets = ptr;
            var dataStart = (byte*)ptr + height * 2;
            var x = 0;
            var y = 0;
            ptr = (ushort*)(dataStart + lineOffsets[0] * 2);
            int minX = width;
            int minY = height;
            int maxX = 0;
            int maxY = 0;

            while (y < height)
            {
                var offsetX = *ptr++;
                var run = *ptr++;

                if (offsetX + run >= 2048)
                {
                    return null;
                }

                if (offsetX + run != 0)
                {
                    x += offsetX;
                    var position = y * width + x;

                    for (var j = 0; j < run; ++j, ++position)
                    {
                        var colorValue = *ptr++;
                        if (colorValue != 0)
                        {
                            pixels[position] = HueHelper.Color16To32(colorValue) | 0xFF_00_00_00;
                        }
                    }

                    x += run;
                }
                else
                {
                    x = 0;
                    ++y;
                    ptr = (ushort*)(dataStart + lineOffsets[y] * 2);
                }
            }

            if (graphic >= 0x2053 && graphic <= 0x2062 || graphic >= 0x206A && graphic <= 0x2079)
            {
                for (var i = 0; i < width; i++)
                {
                    pixels[i] = 0;
                    pixels[(height - 1) * width + i] = 0;
                }

                for (var i = 0; i < height; i++)
                {
                    pixels[i * width] = 0;
                    pixels[i * width + width - 1] = 0;
                }
            }
            /*else if (StaticFilters.IsCave(graphic) && ProfileManager.CurrentProfile != null && ProfileManager.CurrentProfile.EnableCaveBorder)
            {
                for (var yy = 0; yy < height; yy++)
                {
                    var startY = yy != 0 ? -1 : 0;
                    var endY = yy + 1 < height ? 2 : 1;

                    for (var xx = 0; xx < width; xx++)
                    {
                        ref var pixel = ref pixels[yy * width + xx];

                        if (pixel == 0)
                        {
                            continue;
                        }

                        var startX = xx != 0 ? -1 : 0;
                        var endX = xx + 1 < width ? 2 : 1;

                        for (var i = startY; i < endY; i++)
                        {
                            var currentY = yy + i;

                            for (var j = startX; j < endX; j++)
                            {
                                var currentX = xx + j;

                                ref var currentPixel = ref pixels[currentY * width + currentX];

                                if (currentPixel == 0u)
                                {
                                    pixel = 0xFF_00_00_00;
                                }
                            }
                        }
                    }
                }
            }*/

            var pos1 = 0;

            for (y = 0; y < height; ++y)
            {
                for (x = 0; x < width; ++x)
                {
                    if (pixels[pos1++] != 0)
                    {
                        minX = Math.Min(minX, x);
                        maxX = Math.Max(maxX, x);
                        minY = Math.Min(minY, y);
                        maxY = Math.Max(maxY, y);
                    }
                }
            }

            entry.Width = (short)((width >> 1) - 22);
            entry.Height = (short)(height - 44);

            bounds.X = minX;
            bounds.Y = minY;
            bounds.Width = maxX - minX;
            bounds.Height = maxY - minY;

            return pixels;
        }


        private void ReadStaticArt(ref UltimaArtTexture texture, ushort graphic)
        {
            var pixels = ReadStaticArt(graphic, out var width, out var height, out var rect);

            if (pixels != null)
            {
                texture = new UltimaArtTexture(_graphicsDevice, width, height);
                texture.ImageRectangle = rect;
                texture.ApplyData(pixels);
            }
        }

        private unsafe void ReadLandArt(ref UltimaTexture texture, ushort graphic)
        {
            const int SIZE = 44 * 44;

            graphic &= _graphicMask;
            ref var entry = ref GetEntry(graphic);

            if (entry.Length == 0)
            {
                texture = null;

                return;
            }

            _file.Seek(entry.Offset);

            var data = stackalloc uint[SIZE];

            for (var i = 0; i < 22; ++i)
            {
                var start = 22 - (i + 1);
                var pos = i * 44 + start;
                var end = start + ((i + 1) << 1);

                for (var j = start; j < end; ++j)
                {
                    data[pos++] = HueHelper.Color16To32(_file.ReadUInt16()) | 0xFF_00_00_00;
                }
            }

            for (var i = 0; i < 22; ++i)
            {
                var pos = (i + 22) * 44 + i;
                var end = i + ((22 - i) << 1);

                for (var j = i; j < end; ++j)
                {
                    data[pos++] = HueHelper.Color16To32(_file.ReadUInt16()) | 0xFF_00_00_00;
                }
            }

            texture = new UltimaTexture(_graphicsDevice, 44, 44);
            texture.SetDataPointerEXT(0, null, (IntPtr)data, SIZE * sizeof(uint));
        }
    }
}
