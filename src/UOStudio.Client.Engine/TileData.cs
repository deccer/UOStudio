using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UOStudio.Client.Core.Settings;

namespace UOStudio.Client.Engine
{
    public struct LandData
    {
        public unsafe LandData(NewLandTileDataMul mulStruct)
        {
            TextureID = mulStruct.texID;
            Flags = (TileFlag)mulStruct.flags;
            Name = TileDataProvider.ReadNameString(mulStruct.name);
        }

        public unsafe LandData(OldLandTileDataMul mulStruct)
        {
            TextureID = mulStruct.texID;
            Flags = (TileFlag)mulStruct.flags;
            Name = TileDataProvider.ReadNameString(mulStruct.name);
        }

        public string Name { get; set; }

        public ushort TextureID { get; set; }

        public TileFlag Flags { get; set; }

        public void ReadData(string[] split)
        {
            var i = 1;
            Name = split[i++];
            TextureID = (ushort)TileDataProvider.ConvertStringToInt(split[i++]);

            Flags = 0;
            int temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Background;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Weapon;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Transparent;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Translucent;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Wall;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Damaging;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Impassable;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Wet;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unknown1;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Surface;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Bridge;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Generic;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Window;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.NoShoot;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.ArticleA;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.ArticleAn;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.ArticleThe;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Foliage;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.PartialHue;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.NoHouse;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Map;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Container;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Wearable;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.LightSource;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Animation;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.HoverOver;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.NoDiagonal;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Armor;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Roof;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Door;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.StairBack;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.StairRight;
            }

            // Read new flags if file format support them
            //if (!Art.IsUOAHS())
            //{
            //    return;
            //}

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.AlphaBlend;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.UseNewArt;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.ArtUsed;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused8;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.NoShadow;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.PixelBleed;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.PlayAnimOnce;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.MultiMovable;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused10;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused11;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused12;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused13;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused14;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused15;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused16;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused17;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused18;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused19;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused20;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused21;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused22;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused23;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused24;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused25;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused26;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused27;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused28;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused29;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused30;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused31;
            }

            temp = Convert.ToByte(split[i]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused32;
            }
        }
    }

    public struct ItemData
    {
        public unsafe ItemData(NewItemTileDataMul mulStruct)
        {
            Name = TileDataProvider.ReadNameString(mulStruct.name);
            Flags = (TileFlag)mulStruct.flags;
            Weight = mulStruct.weight;
            Quality = mulStruct.quality;
            Quantity = mulStruct.quantity;
            Value = mulStruct.value;
            Height = mulStruct.height;
            Animation = mulStruct.anim;
            Hue = mulStruct.hue;
            StackingOffset = mulStruct.stackingOffset;
            MiscData = mulStruct.miscData;
            Unknown2 = mulStruct.unk2;
            Unknown3 = mulStruct.unk3;
        }

        public unsafe ItemData(OldItemTileDataMul mulStruct)
        {
            Name = TileDataProvider.ReadNameString(mulStruct.name);
            Flags = (TileFlag)mulStruct.flags;
            Weight = mulStruct.weight;
            Quality = mulStruct.quality;
            Quantity = mulStruct.quantity;
            Value = mulStruct.value;
            Height = mulStruct.height;
            Animation = mulStruct.anim;
            Hue = mulStruct.hue;
            StackingOffset = mulStruct.stackingOffset;
            MiscData = mulStruct.miscData;
            Unknown2 = mulStruct.unk2;
            Unknown3 = mulStruct.unk3;
        }

        public string Name { get; set; }

        public short Animation { get; set; }

        public TileFlag Flags { get; set; }

        public bool IsBackground => (Flags & TileFlag.Background) != 0;

        public bool IsBridge => (Flags & TileFlag.Bridge) != 0;

        public bool IsImpassable => (Flags & TileFlag.Impassable) != 0;

        public bool IsSurface => (Flags & TileFlag.Surface) != 0;

        public byte Weight { get; set; }

        public byte Quality { get; set; }

        public byte Quantity { get; set; }

        public byte Value { get; set; }

        public byte Hue { get; set; }

        public byte StackingOffset { get; set; }

        public byte Height { get; set; }

        public short MiscData { get; set; }

        public byte Unknown2 { get; set; }

        public byte Unknown3 { get; set; }

        public int CalculatedHeight => IsBridge ? Height / 2 : Height;

        public bool IsWearable => (Flags & TileFlag.Wearable) != 0;

        public void ReadData(string[] split)
        {
            var i = 1;
            Name = split[i++];
            Weight = Convert.ToByte(split[i++]);
            Quality = Convert.ToByte(split[i++]);
            Animation = (short)TileDataProvider.ConvertStringToInt(split[i++]);
            Height = Convert.ToByte(split[i++]);
            Hue = Convert.ToByte(split[i++]);
            Quantity = Convert.ToByte(split[i++]);
            StackingOffset = Convert.ToByte(split[i++]);
            MiscData = Convert.ToInt16(split[i++]);
            Unknown2 = Convert.ToByte(split[i++]);
            Unknown3 = Convert.ToByte(split[i++]);

            Flags = 0;
            int temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Background;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Weapon;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Transparent;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Translucent;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Wall;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Damaging;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Impassable;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Wet;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unknown1;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Surface;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Bridge;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Generic;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Window;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.NoShoot;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.ArticleA;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.ArticleAn;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.ArticleThe;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Foliage;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.PartialHue;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.NoHouse;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Map;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Container;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Wearable;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.LightSource;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Animation;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.HoverOver;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.NoDiagonal;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Armor;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Roof;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Door;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.StairBack;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.StairRight;
            }

            // Read new flags if file format support them
            //if (!Art.IsUOAHS())
            //{
            //    return;
            //}

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.AlphaBlend;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.UseNewArt;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.ArtUsed;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused8;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.NoShadow;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.PixelBleed;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.PlayAnimOnce;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.MultiMovable;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused10;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused11;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused12;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused13;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused14;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused15;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused16;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused17;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused18;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused19;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused20;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused21;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused22;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused23;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused24;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused25;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused26;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused27;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused28;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused29;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused30;
            }

            temp = Convert.ToByte(split[i++]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused31;
            }

            temp = Convert.ToByte(split[i]);
            if (temp != 0)
            {
                Flags |= TileFlag.Unused32;
            }
        }
    }

    [Flags]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Readability", "RCS1154:Sort enum members.", Justification = "<Pending>")]
    public enum TileFlag : ulong
    {
        None = 0x00000000,
        Background = 0x00000001,
        Weapon = 0x00000002,
        Transparent = 0x00000004,
        Translucent = 0x00000008,
        Wall = 0x00000010,
        Damaging = 0x00000020,
        Impassable = 0x00000040,
        Wet = 0x00000080,
        Unknown1 = 0x00000100,
        Surface = 0x00000200,
        Bridge = 0x00000400,
        Generic = 0x00000800,
        Window = 0x00001000,
        NoShoot = 0x00002000,
        ArticleA = 0x00004000,
        ArticleAn = 0x00008000,
        ArticleThe = 0x00010000,
        Foliage = 0x00020000,
        PartialHue = 0x00040000,
        NoHouse = 0x00080000,
        Map = 0x00100000,
        Container = 0x00200000,
        Wearable = 0x00400000,
        LightSource = 0x00800000,
        Animation = 0x01000000,
        HoverOver = 0x02000000,
        NoDiagonal = 0x04000000,
        Armor = 0x08000000,
        Roof = 0x10000000,
        Door = 0x20000000,
        StairBack = 0x40000000,
        StairRight = 0x80000000,
        AlphaBlend = 0x0100000000,
        UseNewArt = 0x0200000000,
        ArtUsed = 0x0400000000,
        Unused8 = 0x08000000000,
        NoShadow = 0x1000000000,
        PixelBleed = 0x2000000000,
        PlayAnimOnce = 0x4000000000,
        MultiMovable = 0x10000000000,
        Unused10 = 0x20000000000,
        Unused11 = 0x40000000000,
        Unused12 = 0x80000000000,
        Unused13 = 0x100000000000,
        Unused14 = 0x200000000000,
        Unused15 = 0x400000000000,
        Unused16 = 0x800000000000,
        Unused17 = 0x1000000000000,
        Unused18 = 0x2000000000000,
        Unused19 = 0x4000000000000,
        Unused20 = 0x8000000000000,
        Unused21 = 0x10000000000000,
        Unused22 = 0x20000000000000,
        Unused23 = 0x40000000000000,
        Unused24 = 0x80000000000000,
        Unused25 = 0x100000000000000,
        Unused26 = 0x200000000000000,
        Unused27 = 0x400000000000000,
        Unused28 = 0x800000000000000,
        Unused29 = 0x1000000000000000,
        Unused30 = 0x2000000000000000,
        Unused31 = 0x4000000000000000,
        Unused32 = 0x8000000000000000
    }

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
                            HeightTable[i + count] = cur.height;
                        }
                        else
                        {
                            currentPos += sizeof(OldItemTileDataMul);
                            var cur = (OldItemTileDataMul)Marshal.PtrToStructure(ptr, typeof(OldItemTileDataMul));
                            ItemTable[i + count] = new ItemData(cur);
                            HeightTable[i + count] = cur.height;
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


        public static string LandFlagsToString(LandData tile)
        {
            var result = new StringBuilder();
            var enumNames = Enum.GetNames(typeof(TileFlag));
            var enumValues = Enum.GetValues(typeof(TileFlag));
            //int maxLength = Art.IsUOAHS() ? enumValues.Length : (enumValues.Length / 2) + 1;
            var maxLength = enumValues.Length;
            for (int t = 1; t < maxLength; ++t)
            {
                result.AppendLine($"{enumNames[t]}: {((tile.Flags & (TileFlag)enumValues.GetValue(t)) != 0 ? "yes" : "no")}");
            }

            return result.ToString();
        }

        public static string ItemFlagsToString(ItemData tile)
        {
            var result = new StringBuilder();
            var enumNames = Enum.GetNames(typeof(TileFlag));
            var enumValues = Enum.GetValues(typeof(TileFlag));
            //int maxLength = Art.IsUOAHS() ? enumValues.Length : (enumValues.Length / 2) + 1;
            var maxLength = enumValues.Length;
            for (int t = 1; t < maxLength; ++t)
            {
                result.AppendLine($"{enumNames[t]}: {((tile.Flags & (TileFlag)enumValues.GetValue(t)) != 0 ? "yes" : "no")}");
            }

            return result.ToString();
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct OldLandTileDataMul
    {
        public readonly uint flags;
        public readonly ushort texID;
        public fixed byte name[20];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct NewLandTileDataMul
    {
        public readonly ulong flags;
        public readonly ushort texID;
        public fixed byte name[20];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct OldItemTileDataMul
    {
        public readonly uint flags;
        public readonly byte weight;
        public readonly byte quality;
        public readonly short miscData;
        public readonly byte unk2;
        public readonly byte quantity;
        public readonly short anim;
        public readonly byte unk3;
        public readonly byte hue;
        public readonly byte stackingOffset;
        public readonly byte value;
        public readonly byte height;
        public fixed byte name[20];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct NewItemTileDataMul
    {
        public readonly ulong flags;
        public readonly byte weight;
        public readonly byte quality;
        public readonly short miscData;
        public readonly byte unk2;
        public readonly byte quantity;
        public readonly short anim;
        public readonly byte unk3;
        public readonly byte hue;
        public readonly byte stackingOffset;
        public readonly byte value;
        public readonly byte height;
        public fixed byte name[20];
    }
}
