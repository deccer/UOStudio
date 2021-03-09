using System.Globalization;

namespace UOStudio.Client.Engine.Ultima
{
    public static class Utility
    {
        public static bool ToInt32(string value, out int i) =>
            value.StartsWith("0x")
                ? int.TryParse(value.Substring(2), NumberStyles.HexNumber, null, out i)
                : int.TryParse(value, out i);

        public static bool ToUInt32(string value, out uint i) =>
            value.StartsWith("0x")
                ? uint.TryParse(value.Substring(2), NumberStyles.HexNumber, null, out i)
                : uint.TryParse(value, out i);
    }
}
