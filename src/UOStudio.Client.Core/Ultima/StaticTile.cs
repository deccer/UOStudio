using System.Runtime.InteropServices;

namespace UOStudio.Client.Engine.Ultima
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct StaticTile
    {
        internal ushort _id;
        internal byte _x;
        internal byte _y;
        internal sbyte _z;
        internal short _hue;

        public ushort Id => _id;

        public int X
        {
            get => _x;
            set => _x = (byte)value;
        }

        public int Y
        {
            get => _y;
            set => _y = (byte)value;
        }

        public int Z
        {
            get => _z;
            set => _z = (sbyte)value;
        }

        public int Hue
        {
            get => _hue;
            set => _hue = (short)value;
        }

        public int Height => TileData.ItemTable[_id & TileData.MaxItemValue].Height;

        public StaticTile(ushort id, sbyte z)
        {
            _id = id;
            _z = z;

            _x = 0;
            _y = 0;
            _hue = 0;
        }

        public StaticTile(ushort id, byte x, byte y, sbyte z, short hue)
        {
            _id = id;
            _x = x;
            _y = y;
            _z = z;
            _hue = hue;
        }

        public void Set(ushort id, sbyte z)
        {
            _id = id;
            _z = z;
        }

        public void Set(ushort id, byte x, byte y, sbyte z, short hue)
        {
            _id = id;
            _x = x;
            _y = y;
            _z = z;
            _hue = hue;
        }
    }
}
