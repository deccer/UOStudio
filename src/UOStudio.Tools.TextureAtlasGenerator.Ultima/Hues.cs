using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace UOStudio.Tools.TextureAtlasGenerator.Ultima
{
    public static class Hues
    {
        private static int[] _header;

        public static Hue[] List { get; private set; }

        static Hues()
        {
            Initialize();
        }

        /// <summary>
        /// Reads hues.mul and fills <see cref="List"/>
        /// </summary>
        public static void Initialize()
        {
            var path = Files.GetFilePath("hues.mul");
            var index = 0;

            const int maxHueCount = 3000;
            List = new Hue[maxHueCount];

            if (path != null)
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var blockCount = (int)fs.Length / 708;

                    if (blockCount > 375)
                    {
                        blockCount = 375;
                    }

                    _header = new int[blockCount];
                    var structSize = Marshal.SizeOf(typeof(HueDataMul));
                    var buffer = new byte[blockCount * (4 + (8 * structSize))];
                    var gc = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    try
                    {
                        fs.Read(buffer, 0, buffer.Length);
                        long currentPos = 0;

                        for (var i = 0; i < blockCount; ++i)
                        {
                            var ptrHeader = new IntPtr((long)gc.AddrOfPinnedObject() + currentPos);
                            currentPos += 4;
                            _header[i] = (int)Marshal.PtrToStructure(ptrHeader, typeof(int));

                            for (var j = 0; j < 8; ++j, ++index)
                            {
                                var ptr = new IntPtr((long)gc.AddrOfPinnedObject() + currentPos);
                                currentPos += structSize;
                                var cur = (HueDataMul)Marshal.PtrToStructure(ptr, typeof(HueDataMul));
                                List[index] = new Hue(index, cur);
                            }
                        }
                    }
                    finally
                    {
                        gc.Free();
                    }
                }
            }

            for (; index < List.Length; ++index)
            {
                List[index] = new Hue(index);
            }
        }

        public static void Save(string path)
        {
            var mul = Path.Combine(path, "hues.mul");
            using (var fsMul = new FileStream(mul, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (var binMul = new BinaryWriter(fsMul))
            {
                var index = 0;
                foreach (var blockIdx in _header)
                {
                    binMul.Write(blockIdx);
                    for (var j = 0; j < 8; ++j, ++index)
                    {
                        for (var colorIndex = 0; colorIndex < 32; ++colorIndex)
                        {
                            binMul.Write((short)(List[index].Colors[colorIndex] ^ 0x8000));
                        }

                        binMul.Write((short)(List[index].TableStart ^ 0x8000));
                        binMul.Write((short)(List[index].TableEnd ^ 0x8000));

                        var nameBuffer = new byte[20];
                        if (List[index].Name != null)
                        {
                            var bytes = Encoding.ASCII.GetBytes(List[index].Name);
                            if (bytes.Length > 20)
                            {
                                Array.Resize(ref bytes, 20);
                            }

                            bytes.CopyTo(nameBuffer, 0);
                        }

                        binMul.Write(nameBuffer);
                    }
                }
            }
        }

        /// <summary>
        /// Returns <see cref="Hue"/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Hue GetHue(int index)
        {
            index &= 0x3FFF;

            if (index >= 0 && index < 3000)
            {
                return List[index];
            }

            return List[0];
        }

        /// <summary>
        /// Converts RGB value to Hue color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static short ColorToHue(Color color)
        {
            const double scale = 31.0 / 255;

            ushort origRed = color.R;
            var newRed = (ushort)(origRed * scale);
            if (newRed == 0 && origRed != 0)
            {
                newRed = 1;
            }

            ushort origGreen = color.G;
            var newGreen = (ushort)(origGreen * scale);
            if (newGreen == 0 && origGreen != 0)
            {
                newGreen = 1;
            }

            ushort origBlue = color.B;
            var newBlue = (ushort)(origBlue * scale);
            if (newBlue == 0 && origBlue != 0)
            {
                newBlue = 1;
            }

            return (short)((newRed << 10) | (newGreen << 5) | newBlue);
        }

        /// <summary>
        /// Converts Hue color to RGB color
        /// </summary>
        /// <param name="hue"></param>
        /// <returns></returns>
        public static Color HueToColor(short hue)
        {
            const int scale = 255 / 31;
            return Color.FromArgb(((hue & 0x7c00) >> 10) * scale, ((hue & 0x3e0) >> 5) * scale, (hue & 0x1f) * scale);
        }

        public static int HueToColorR(short hue)
        {
            return ((hue & 0x7c00) >> 10) * (255 / 31);
        }

        public static int HueToColorG(short hue)
        {
            return ((hue & 0x3e0) >> 5) * (255 / 31);
        }

        public static int HueToColorB(short hue)
        {
            return (hue & 0x1f) * (255 / 31);
        }

        public static unsafe void ApplyTo(Bitmap bmp, short[] colors, bool onlyHueGrayPixels)
        {
            var bd = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format16bppArgb1555);

            var stride = bd.Stride >> 1;
            var width = bd.Width;
            var height = bd.Height;
            var delta = stride - width;

            var pBuffer = (ushort*)bd.Scan0;
            var pLineEnd = pBuffer + width;
            var pImageEnd = pBuffer + (stride * height);

            if (onlyHueGrayPixels)
            {
                while (pBuffer < pImageEnd)
                {
                    while (pBuffer < pLineEnd)
                    {
                        int c = *pBuffer;
                        if (c != 0)
                        {
                            var r = (c >> 10) & 0x1F;
                            var g = (c >> 5) & 0x1F;
                            var b = c & 0x1F;
                            if (r == g && r == b)
                            {
                                *pBuffer = (ushort)colors[(c >> 10) & 0x1F];
                            }
                        }
                        ++pBuffer;
                    }

                    pBuffer += delta;
                    pLineEnd += stride;
                }
            }
            else
            {
                while (pBuffer < pImageEnd)
                {
                    while (pBuffer < pLineEnd)
                    {
                        if (*pBuffer != 0)
                        {
                            *pBuffer = (ushort)colors[(*pBuffer >> 10) & 0x1F];
                        }

                        ++pBuffer;
                    }

                    pBuffer += delta;
                    pLineEnd += stride;
                }
            }

            bmp.UnlockBits(bd);
        }
    }

    public sealed class Hue
    {
        public int Index { get; }
        public short[] Colors { get; }
        public string Name { get; set; }
        public short TableStart { get; set; }
        public short TableEnd { get; set; }

        public Hue(int index)
        {
            Name = "Null";
            Index = index;
            Colors = new short[32];
            TableStart = 0;
            TableEnd = 0;
        }

        public Color GetColor(int index)
        {
            return Hues.HueToColor(Colors[index]);
        }

        private static readonly byte[] _stringBuffer = new byte[20];

        public Hue(int index, BinaryReader bin)
        {
            Index = index;
            Colors = new short[32];

            var buffer = bin.ReadBytes(88);
            unsafe
            {
                fixed (byte* bufferPtr = buffer)
                {
                    var buf = (ushort*)bufferPtr;
                    for (var i = 0; i < 32; ++i)
                    {
                        Colors[i] = (short)(*buf++ | 0x8000);
                    }

                    TableStart = (short)(*buf++ | 0x8000);
                    TableEnd = (short)(*buf++ | 0x8000);
                    var stringBuffer = (byte*)buf;
                    int count;
                    for (count = 0; count < 20 && *stringBuffer != 0; ++count)
                    {
                        _stringBuffer[count] = *stringBuffer++;
                    }

                    Name = Encoding.ASCII.GetString(_stringBuffer, 0, count);
                    Name = Name.Replace("\n", " ");
                }
            }
        }

        public Hue(int index, HueDataMul mulStruct)
        {
            Index = index;
            Colors = new short[32];
            for (var i = 0; i < 32; ++i)
            {
                Colors[i] = (short)(mulStruct.colors[i] | 0x8000);
            }

            TableStart = (short)(mulStruct.tableStart | 0x8000);
            TableEnd = (short)(mulStruct.tableEnd | 0x8000);

            Name = NativeMethods.ReadNameString(mulStruct.name, 20);
            Name = Name.Replace("\n", " ");
        }

        /// <summary>
        /// Applies Hue to Bitmap
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="onlyHueGrayPixels"></param>
        public unsafe void ApplyTo(Bitmap bmp, bool onlyHueGrayPixels)
        {
            var bd = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format16bppArgb1555);

            var stride = bd.Stride >> 1;
            var width = bd.Width;
            var height = bd.Height;
            var delta = stride - width;

            var pBuffer = (ushort*)bd.Scan0;
            var pLineEnd = pBuffer + width;
            var pImageEnd = pBuffer + (stride * height);

            if (onlyHueGrayPixels)
            {
                while (pBuffer < pImageEnd)
                {
                    while (pBuffer < pLineEnd)
                    {
                        int c = *pBuffer;
                        if (c != 0)
                        {
                            var r = (c >> 10) & 0x1F;
                            var g = (c >> 5) & 0x1F;
                            var b = c & 0x1F;
                            if (r == g && r == b)
                            {
                                *pBuffer = (ushort)Colors[(c >> 10) & 0x1F];
                            }
                        }
                        ++pBuffer;
                    }

                    pBuffer += delta;
                    pLineEnd += stride;
                }
            }
            else
            {
                while (pBuffer < pImageEnd)
                {
                    while (pBuffer < pLineEnd)
                    {
                        if (*pBuffer != 0)
                        {
                            *pBuffer = (ushort)Colors[(*pBuffer >> 10) & 0x1F];
                        }

                        ++pBuffer;
                    }

                    pBuffer += delta;
                    pLineEnd += stride;
                }
            }

            bmp.UnlockBits(bd);
        }

        public void Export(string fileName)
        {
            using (var tex = new StreamWriter(new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite), Encoding.GetEncoding(1252)))
            {
                tex.WriteLine(Name);
                tex.WriteLine(((short)(TableStart ^ 0x8000)).ToString());
                tex.WriteLine(((short)(TableEnd ^ 0x8000)).ToString());

                foreach (var colorValue in Colors)
                {
                    tex.WriteLine(((short)(colorValue ^ 0x8000)).ToString());
                }
            }
        }

        public void Import(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return;
            }

            using (var sr = new StreamReader(fileName))
            {
                string line;
                var i = -3;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    try
                    {
                        if (i >= Colors.Length)
                        {
                            break;
                        }

                        switch (i)
                        {
                            case -3:
                                Name = line;
                                break;
                            case -2:
                                TableStart = (short)(ushort.Parse(line) | 0x8000);
                                break;
                            case -1:
                                TableEnd = (short)(ushort.Parse(line) | 0x8000);
                                break;
                            default:
                                Colors[i] = (short)(ushort.Parse(line) | 0x8000);
                                break;
                        }
                        ++i;
                    }
                    catch
                    {
                        // TODO: ignored?
                        // ignored
                    }
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct HueDataMul
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public readonly ushort[] colors;
        public readonly ushort tableStart;
        public readonly ushort tableEnd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public readonly byte[] name;
    }
}