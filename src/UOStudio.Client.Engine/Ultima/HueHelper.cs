using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace UOStudio.Client.Engine.Ultima
{
    internal static class HueHelper
    {
        private static readonly byte[] _table = new byte[32]
        {
            0x00, 0x08, 0x10, 0x18, 0x20, 0x29, 0x31, 0x39, 0x41, 0x4A, 0x52, 0x5A, 0x62, 0x6A, 0x73, 0x7B, 0x83, 0x8B,
            0x94, 0x9C, 0xA4, 0xAC, 0xB4, 0xBD, 0xC5, 0xCD, 0xD5, 0xDE, 0xE6, 0xEE, 0xF6, 0xFF
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (byte, byte, byte, byte) GetBgra(uint color) =>
            ((byte)(color & 0xFF),
                (byte)((color >> 8) & 0xFF),
                (byte)((color >> 16) & 0xFF),
                (byte)((color >> 24) & 0xFF));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RgbaToArgb(uint rgba) => (rgba >> 8) | (rgba << 24);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Color16To32(ushort color) => (uint)(_table[(color >> 10) & 0x1F] | (_table[(color >> 5) & 0x1F] << 8) | (_table[color & 0x1F] << 16));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Color32To16(uint color) =>
            (ushort)((((color & 0xFF) << 5) >> 8) | (((((color >> 16) & 0xFF) << 5) >> 8) << 10) |
                     (((((color >> 8) & 0xFF) << 5) >> 8) << 5));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ConvertToGray(ushort color) => (ushort)(((color & 0x1F) * 299 + ((color >> 5) & 0x1F) * 587 + ((color >> 10) & 0x1F) * 114) / 1000);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ColorToHue(Color color)
        {
            ushort red = color.R;
            ushort green = color.G;
            ushort blue = color.B;
            const double scale = 31.0 / 255;

            var hueRed = (ushort)(red * scale);

            if (hueRed == 0 && red != 0)
            {
                hueRed = 1;
            }

            var hueGreen = (ushort)(green * scale);

            if (hueGreen == 0 && green != 0)
            {
                hueGreen = 1;
            }

            var hueBlue = (ushort)(blue * scale);

            if (hueBlue == 0 && blue != 0)
            {
                hueBlue = 1;
            }

            var hue = (ushort)((hueRed << 10) | (hueGreen << 5) | hueBlue);

            return hue;
        }
    }
}
