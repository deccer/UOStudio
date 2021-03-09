using System;
using System.Runtime.InteropServices;

namespace UOStudio.Common.Core.IO
{
    public static class NativeReader
    {
        private static readonly INativeReader _nativeReader;

        static NativeReader() =>
            _nativeReader = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? (INativeReader)new NativeReaderWin32()
                : new NativeReaderUnix();

        public static unsafe void Read(IntPtr fileHandle, void* buffer, int length)
        {
            _nativeReader.Read(fileHandle, buffer, length);
        }
    }
}
