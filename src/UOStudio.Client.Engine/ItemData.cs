using System;
using System.ComponentModel;

namespace UOStudio.Client.Engine
{
    public struct ItemData
    {
        public unsafe ItemData(NewItemTileDataMul mulStruct)
        {
            Name = TileDataProvider.ReadNameString(mulStruct.Name);
            Flags = (TileFlag)mulStruct.Flags;
            Weight = mulStruct.Weight;
            Quality = mulStruct.Quality;
            Quantity = mulStruct.Quantity;
            Value = mulStruct.Value;
            Height = mulStruct.Height;
            Animation = mulStruct.Anim;
            Hue = mulStruct.Hue;
            StackingOffset = mulStruct.StackingOffset;
            MiscData = mulStruct.MiscData;
            Unknown2 = mulStruct.Unknown2;
            Unknown3 = mulStruct.Unknown3;
        }

        public unsafe ItemData(OldItemTileDataMul mulStruct)
        {
            Name = TileDataProvider.ReadNameString(mulStruct.Name);
            Flags = (TileFlag)mulStruct.Flags;
            Weight = mulStruct.Weight;
            Quality = mulStruct.Quality;
            Quantity = mulStruct.Quantity;
            Value = mulStruct.Value;
            Height = mulStruct.Height;
            Animation = mulStruct.Anim;
            Hue = mulStruct.Hue;
            StackingOffset = mulStruct.StackingOffset;
            MiscData = mulStruct.MiscData;
            Unknown2 = mulStruct.Unknown2;
            Unknown3 = mulStruct.Unknown3;
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
}
