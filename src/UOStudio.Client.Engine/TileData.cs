using System.Runtime.InteropServices;

namespace UOStudio.Client.Engine
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct OldLandTileDataMul
    {
        public readonly uint Flags;
        public readonly ushort TexID;
        public fixed byte name[20];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct NewLandTileDataMul
    {
        public readonly ulong Flags;
        public readonly ushort TexID;
        public fixed byte Name[20];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct OldItemTileDataMul
    {
        public readonly uint Flags;
        public readonly byte Weight;
        public readonly byte Quality;
        public readonly short MiscData;
        public readonly byte Unknown2;
        public readonly byte Quantity;
        public readonly short Anim;
        public readonly byte Unknown3;
        public readonly byte Hue;
        public readonly byte StackingOffset;
        public readonly byte Value;
        public readonly byte Height;
        public fixed byte Name[20];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct NewItemTileDataMul
    {
        public readonly ulong Flags;
        public readonly byte Weight;
        public readonly byte Quality;
        public readonly short MiscData;
        public readonly byte Unknown2;
        public readonly byte Quantity;
        public readonly short Anim;
        public readonly byte Unknown3;
        public readonly byte Hue;
        public readonly byte StackingOffset;
        public readonly byte Value;
        public readonly byte Height;
        public fixed byte Name[20];
    }
}
