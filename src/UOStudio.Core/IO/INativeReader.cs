using System;

namespace UOStudio.Core.IO
{
    public interface INativeReader
    {
        unsafe void Read(IntPtr fileHandle, void* buffer, int length);
    }
}
