using System;

namespace UOStudio.Client.Engine.Ultima
{
    public struct UltimaFileIndex
    {
        public UltimaFileIndex
        (
            IntPtr address,
            uint fileSize,
            long offset,
            int length,
            int decompressedLength,
            short width = 0,
            short height = 0,
            ushort hue = 0
        )
        {
            Address = address;
            FileSize = fileSize;
            Offset = offset;
            Length = length;
            DecompressedLength = decompressedLength;
            Width = width;
            Height = height;
            Hue = hue;

            AnimationOffset = 0;
        }

        public IntPtr Address;
        public uint FileSize;
        public long Offset;
        public int Length;
        public int DecompressedLength;
        public short Width;
        public short Height;
        public ushort Hue;
        public sbyte AnimationOffset;

        public static UltimaFileIndex Invalid = new UltimaFileIndex(IntPtr.Zero, 0, 0, 0, 0);
    }
}
