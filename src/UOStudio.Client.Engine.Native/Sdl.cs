using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace UOStudio.Client.Engine.Native
{
    [SuppressMessage("ReSharper", "S3963")]
    public static unsafe partial class Sdl
    {
        /*********************************************************************************************/

        private const int WindowPositionCentered = 0x2FFF0000;

        public const int TexteditingEventTextSize = 32;
        public const int TextInputeventTextSize = 32;

        public const int ScanCodeMask = 1 << 30;

        private static uint SdlButton(uint x)
        {
            return (uint)(1 << ((int)x - 1));
        }

        public const uint ButtonLeft = 1;
        public const uint ButtonMiddle = 2;
        public const uint ButtonRight = 3;
        public const uint ButtonX1 = 4;
        public const uint ButtonX2 = 5;
        public static readonly uint ButtonLeftMask = SdlButton(ButtonLeft);
        public static readonly uint ButtonMiddleMask = SdlButton(ButtonMiddle);
        public static readonly uint ButtonRightMask = SdlButton(ButtonRight);
        public static readonly uint X1Mask = SdlButton(ButtonX1);
        public static readonly uint X2Mask = SdlButton(ButtonX2);

        public static KeyCode ScancodeToKeycode(Scancode x)
        {
            return (KeyCode)((int)x | ScanCodeMask);
        }

        public static int Init(InitFlags initFlags)
        {
            return _sdlInitFnPtr(initFlags);
        }

        public static void Quit()
        {
            _sdlQuitFnPtr();
        }

        public static Rectangle GetDisplayBounds(int displayIndex)
        {
            if (_sdlGetDisplayBounds(displayIndex, out var rectangle) == 0)
            {
                return rectangle;
            }

            throw new Exception("SDL: Unable to get the display bounds");
        }

        public static IntPtr CreateWindow(string windowTitle, int width, int height, WindowFlags flags)
        {
            var title = Marshal.StringToHGlobalAnsi(windowTitle);
            var result = _sdlCreateWindowDelegate(
                title,
                WindowPositionCentered,
                WindowPositionCentered,
                width,
                height,
                flags);
            Marshal.FreeHGlobal(title);
            return result;
        }

        public static void DestroyWindow(IntPtr windowHandle)
        {
            _sdlDestroyWindowDelegate(windowHandle);
        }

        public static int PollEvent(out SdlEvent ev)
        {
            fixed (SdlEvent* e = &ev)
            {
                return _sdlPollEventDelegate(e);
            }
        }

        public static IntPtr CreateRenderContext(IntPtr windowHandle)
        {
            return _sdlGlCreateContextDelegate(windowHandle);
        }

        public static void MakeCurrent(IntPtr windowHandle, IntPtr renderContext)
        {
            _sdlGlMakeCurrentDelegate(windowHandle, renderContext);
        }

        public static void DeleteRenderContext(IntPtr renderContext)
        {
            _sdlGlDeleteContextDelegate(renderContext);
        }

        public static void SwapWindow(IntPtr windowHandle)
        {
            _sdlGlSwapWindowDelegate(windowHandle);
        }

        public static void SetGlAttribute(GlAttribute attribute, int value)
        {
            _sdlGlSetAttributeDelegate(attribute, value);
        }

        public static void SetGlAttribute(GlAttribute attribute, SdlProfile profile)
        {
            _sdlGlSetAttributeProfileDelegate(attribute, profile);
        }

        public static void SetGlAttribute(GlAttribute attribute, SdlContext context)
        {
            _sdlGlSetAttributeContextDelegate(attribute, context);
        }

        public static IntPtr GetProcAddress(string procName)
        {
            var procNamePtr = Marshal.StringToHGlobalAnsi(procName);
            var address = _sdlGlGetProcAddressDelegate(procNamePtr);
            Marshal.ZeroFreeGlobalAllocAnsi(procNamePtr);
            return address;
        }

        public static uint GetGlobalMouseState(out int x, out int y)
        {
            var mx = default(IntPtr);
            var my = default(IntPtr);
            var flags = _sdlGetGlobalMouseState(mx, my);
            x = mx.ToInt32();
            y = my.ToInt32();
            return flags;
        }

        public static uint GetRelativeMouseState(out int x, out int y)
        {
            return _sdlGetRelativeMouseState(out x, out y);
        }

        public static string GetWindowTitle(IntPtr windowHandle)
        {
            return Marshal.PtrToStringAnsi(_sdlGetWindowTitle(windowHandle));
        }

        public static void SetWindowTitle(IntPtr windowHandle, string windowTitle)
        {
            var windowTitlePtr = Marshal.StringToHGlobalAnsi(windowTitle);
            _sdlSetWindowTitle(windowHandle, (byte*)windowTitlePtr);
            Marshal.FreeHGlobal(windowTitlePtr);
        }

        public static bool GetRelativeMouseMode()
        {
            return _sdlGetRelativeMouseMode() == SdlBool.True;
        }

        public static void GetWindowPosition(IntPtr windowHandle, out int x, out int y)
        {
            _sdlGetWindowPosition(windowHandle, out x, out y);
        }

        public static uint GetMouseState(out int x, out int y)
        {
            return _sdlGetMouseState(out x, out y);
        }

        public static void SetMousePosition(IntPtr windowHandle, int x, int y)
        {
            _sdlWarpMouseInWindow(windowHandle, x, y);
        }

        public static IntPtr GetKeyboardState(out int numberOfKeys)
        {
            return _sdlGetKeyboardState(out numberOfKeys);
        }

        public static Scancode ScanCodeFromKeycode(KeyCode keyCode)
        {
            return _sdlScanCodeFromKeycode(keyCode);
        }

        public static void PumpEvents()
        {
            _sdlPumpEvents();
        }

        public static int SetSwapInterval(int interval)
        {
            return _sdlGlSetSwapInterval(interval);
        }

        public static void GetWindowSize(IntPtr windowHandle, out int width, out int height)
        {
            _sdlGetWindowSize(windowHandle, out width, out height);
        }
    }
}
