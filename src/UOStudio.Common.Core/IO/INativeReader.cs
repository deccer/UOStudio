using System;

namespace UOStudio.Common.Core.IO
{
    public interface INativeReader
    {
        unsafe void Read(IntPtr fileHandle, void* buffer, int length);
    }
}
