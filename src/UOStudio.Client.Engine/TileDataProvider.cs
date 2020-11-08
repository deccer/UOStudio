using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UOStudio.Client.Core.Settings;

namespace UOStudio.Client.Engine
{
    public class TileDataProvider
    {
        private readonly IAppSettingsProvider _appSettingsProvider;
        private readonly bool _isUoahs;
        public LandData[] LandTable { get; private set; }

        public ItemData[] ItemTable { get; private set; }

        public int[] HeightTable { get; private set; }

        public static unsafe string ReadNameString(byte* buffer)
        {
            byte[] _stringBuffer = new byte[20];
            int count;
            for (count = 0; count < 20 && *buffer != 0; ++count)
            {
                _stringBuffer[count] = *buffer++;
            }

            return Encoding.Default.GetString(_stringBuffer, 0, count);
        }

        private int[] _landHeader;
        private int[] _itemHeader;

        public TileDataProvider(IAppSettingsProvider appSettingsProvider, bool isUOAHS)
        {
            _appSettingsProvider = appSettingsProvider;
            _isUoahs = isUOAHS;
            Initialize();
        }

        public unsafe void Initialize()
        {
            var filePath = Path.Combine(_appSettingsProvider.AppSettings.General.UltimaOnlineBasePath, "tiledata.mul");

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            bool useNeWTileDataFormat = _isUoahs;
            _landHeader = new int[512];
            var j = 0;
            LandTable = new LandData[0x4000];

            var buffer = new byte[fs.Length];
            var gc = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            long currentPos = 0;
            try
            {
                fs.Read(buffer, 0, buffer.Length);
                for (var i = 0; i < 0x4000; i += 32)
                {
                    var ptrHeader = new IntPtr((long)gc.AddrOfPinnedObject() + currentPos);
                    currentPos += 4;
                    _landHeader[j++] = (int)Marshal.PtrToStructure(ptrHeader, typeof(int));
                    for (var count = 0; count < 32; ++count)
                    {
                        var ptr = new IntPtr((long)gc.AddrOfPinnedObject() + currentPos);
                        if (useNeWTileDataFormat)
                        {
                            currentPos += sizeof(NewLandTileDataMul);
                            var cur = (NewLandTileDataMul)Marshal.PtrToStructure(ptr, typeof(NewLandTileDataMul));
                            LandTable[i + count] = new LandData(cur);
                        }
                        else
                        {
                            currentPos += sizeof(OldLandTileDataMul);
                            var cur = (OldLandTileDataMul)Marshal.PtrToStructure(ptr, typeof(OldLandTileDataMul));
                            LandTable[i + count] = new LandData(cur);
                        }
                    }
                }

                var remaining = buffer.Length - currentPos;

                var structSize = useNeWTileDataFormat ? sizeof(NewItemTileDataMul) : sizeof(OldItemTileDataMul);

                _itemHeader = new int[remaining / ((structSize * 32) + 4)];
                var itemLength = _itemHeader.Length * 32;

                ItemTable = new ItemData[itemLength];
                HeightTable = new int[itemLength];

                j = 0;
                for (var i = 0; i < itemLength; i += 32)
                {
                    var ptrHeader = new IntPtr((long)gc.AddrOfPinnedObject() + currentPos);
                    currentPos += 4;
                    _itemHeader[j++] = (int)Marshal.PtrToStructure(ptrHeader, typeof(int));
                    for (var count = 0; count < 32; ++count)
                    {
                        var ptr = new IntPtr((long)gc.AddrOfPinnedObject() + currentPos);
                        if (useNeWTileDataFormat)
                        {
                            currentPos += sizeof(NewItemTileDataMul);
                            var cur = (NewItemTileDataMul)Marshal.PtrToStructure(ptr, typeof(NewItemTileDataMul));
                            ItemTable[i + count] = new ItemData(cur);
                            HeightTable[i + count] = cur.Height;
                        }
                        else
                        {
                            currentPos += sizeof(OldItemTileDataMul);
                            var cur = (OldItemTileDataMul)Marshal.PtrToStructure(ptr, typeof(OldItemTileDataMul));
                            ItemTable[i + count] = new ItemData(cur);
                            HeightTable[i + count] = cur.Height;
                        }
                    }
                }
            }
            finally
            {
                gc.Free();
            }
        }

        public static int ConvertStringToInt(string text)
        {
            int result;
            if (text.Contains("0x"))
            {
                var convert = text.Replace("0x", "");
                int.TryParse(convert, NumberStyles.HexNumber, null, out result);
            }
            else
            {
                int.TryParse(text, NumberStyles.Integer, null, out result);
            }

            return result;
        }

        public static IReadOnlyCollection<string> LandFlagsToPropertyList(LandData tile)
        {
            var result = new List<string>();
            var enumNames = Enum.GetNames(typeof(TileFlag));
            var enumValues = Enum.GetValues(typeof(TileFlag));

            var maxLength = enumValues.Length;
            for (int t = 1; t < maxLength; ++t)
            {
                result.Add(enumNames[t]);
                result.Add($"{((tile.Flags & (TileFlag)enumValues.GetValue(t)) != 0 ? "yes" : "no")}");
            }

            return result;
        }

        public static IReadOnlyCollection<string> ItemFlagsToPropertyList(ItemData tile)
        {
            var result = new List<string>();
            var enumNames = Enum.GetNames(typeof(TileFlag));
            var enumValues = Enum.GetValues(typeof(TileFlag));

            var maxLength = enumValues.Length;
            for (int t = 1; t < maxLength; ++t)
            {
                result.Add(enumNames[t]);
                result.Add($"{((tile.Flags & (TileFlag)enumValues.GetValue(t)) != 0 ? "yes" : "no")}");
            }

            return result;
        }
    }
}
