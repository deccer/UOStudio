using System;
using System.Runtime.InteropServices;

namespace UOStudio.Common.Core.IO
{
    internal sealed class NativeReaderUnix : INativeReader
    {
        public unsafe void Read(IntPtr fileHandle, void* buffer, int length)
        {
            _ = UnsafeNativeMethods.read(fileHandle, buffer, length);
        }

        internal static class UnsafeNativeMethods
        {
            [DllImport("libc")]
            internal static extern unsafe int read(IntPtr fileHandle, void* buffer, int length);
        }
    }
}
