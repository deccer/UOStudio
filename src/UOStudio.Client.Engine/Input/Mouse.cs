using UOStudio.Client.Engine.Native;

namespace UOStudio.Client.Engine.Input
{
    public static class Mouse
    {
        public static readonly bool SupportsGlobalMouse = false;

        internal static int InternalMouseWheel;

        public static IntPtr WindowHandle
        {
            get;
            set;
        }

        public static MouseState GetState()
        {
            GetMouseState(
                WindowHandle,
                out var x,
                out var y,
                out var left,
                out var middle,
                out var right,
                out var x1,
                out var x2,
                out var x3,
                out var x4
            );
            return new MouseState(x, y, InternalMouseWheel, left, middle, right, x1, x2, x3, x4);
        }

        private static void GetMouseState(
            IntPtr window,
            out int x,
            out int y,
            out ButtonState left,
            out ButtonState middle,
            out ButtonState right,
            out ButtonState x1,
            out ButtonState x2,
            out ButtonState x3,
            out ButtonState x4
        )
        {
            uint flags;
            if (Sdl.GetRelativeMouseMode())
            {
                flags = Sdl.GetRelativeMouseState(out x, out y);
            }
            else if (SupportsGlobalMouse)
            {
                flags = Sdl.GetGlobalMouseState(out x, out y);
                Sdl.GetWindowPosition(window, out var wx, out var wy);
                x -= wx;
                y -= wy;
            }
            else
            {
                /* This is inaccurate, but what can you do... */
                flags = Sdl.GetMouseState(out x, out y);
            }
            left = (ButtonState)(flags & Sdl.ButtonLeftMask);
            middle = (ButtonState)((flags & Sdl.ButtonMiddleMask) >> 1);
            right = (ButtonState)((flags & Sdl.ButtonRightMask) >> 2);
            x1 = (ButtonState)((flags & Sdl.X1Mask) >> 3);
            x2 = (ButtonState)((flags & Sdl.X2Mask) >> 4);
            x3 = (ButtonState)((flags & Sdl.X2Mask) >> 5);
            x4 = (ButtonState)((flags & Sdl.X2Mask) >> 6);
        }

        public static void SetPosition(int x, int y)
        {
            Sdl.SetMousePosition(WindowHandle, x, y);
        }
    }
}
