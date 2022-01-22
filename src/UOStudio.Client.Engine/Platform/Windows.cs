using System.Runtime.InteropServices;

namespace UOStudio.Client.Engine.Platform
{
    public static class Windows
    {
        [DllImport("SHCore.dll", SetLastError = true)]
        public static extern bool SetProcessDpiAwareness(ProcessDpiAwareness awareness);

        [DllImport("SHCore.dll", SetLastError = true)]
        public static extern void GetProcessDpiAwareness(IntPtr processHandle, out ProcessDpiAwareness awareness);

        public enum ProcessDpiAwareness
        {
            Unaware = 0,
            SystemDpiAware = 1,
            PerMonitorDpiAware = 2
        }
    }
}