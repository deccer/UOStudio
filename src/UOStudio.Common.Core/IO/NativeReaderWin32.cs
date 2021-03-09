using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace UOStudio.Common.Core.IO
{
    internal sealed class NativeReaderWin32 : INativeReader
    {
        public unsafe void Read(IntPtr fileHandle, void* buffer, int length)
        {
            uint lpNumberOfBytesRead = 0;
            UnsafeNativeMethods.ReadFile(fileHandle, buffer, (uint)length, ref lpNumberOfBytesRead, null);
        }

        internal static class UnsafeNativeMethods
        {
            [DllImport("kernel32")]
            internal static extern unsafe bool ReadFile(
                IntPtr hFile, void* lpBuffer, uint nNumberOfBytesToRead,
                ref uint lpNumberOfBytesRead, NativeOverlapped* lpOverlapped
            );
        }
    }
}
