using System;
using System.Runtime.InteropServices;

namespace UOStudio.Common.Core.Extensions
{
    public static class ByteArrayExtensions
    {
        public static unsafe T RawDeserializer<T>(byte[] bytes) where T : struct
        {
            _ = bytes ?? throw new ArgumentNullException(nameof(bytes));

            fixed (byte* bytesPtr = bytes)
            {
                return Marshal.PtrToStructure<T>((IntPtr)bytesPtr);
            }
        }
    }
}
