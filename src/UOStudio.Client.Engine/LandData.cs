using System;

namespace UOStudio.Client.Engine
{
    public struct LandData
    {
        public unsafe LandData(NewLandTileDataMul mulStruct)
        {
            TextureID = mulStruct.TexID;
            Flags = (TileFlag)mulStruct.Flags;
            Name = TileDataProvider.ReadNameString(mulStruct.Name);
        }

        public unsafe LandData(OldLandTileDataMul mulStruct)
        {
            TextureID = mulStruct.TexID;
            Flags = (TileFlag)mulStruct.Flags;
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
