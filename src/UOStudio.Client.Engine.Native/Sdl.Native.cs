using System;
using System.Runtime.InteropServices;

// ReSharper disable S101

namespace UOStudio.Client.Engine.Native
{
    public static unsafe partial class Sdl
    {
        private const string SdlLibrary = "SDL2";

        private static readonly delegate* <InitFlags, int> _sdlInitFnPtr = &SDL_Init;
        private static readonly delegate* <void> _sdlQuitFnPtr = &SDL_Quit;

        private static readonly delegate* <IntPtr, int, int, int, int, WindowFlags, IntPtr> _sdlCreateWindowDelegate = &SDL_CreateWindow;
        private static readonly delegate* <IntPtr, void> _sdlDestroyWindowDelegate = &SDL_DestroyWindow;
        private static readonly delegate* <SdlEvent*, int> _sdlPollEventDelegate = &SDL_PollEvent;

        private static readonly delegate* <int, out Rectangle, int> _sdlGetDisplayBounds = &SDL_GetDisplayBounds;

        private static readonly delegate* <IntPtr, IntPtr> _sdlGlCreateContextDelegate = &SDL_GL_CreateContext;
        private static readonly delegate* <IntPtr, IntPtr, void> _sdlGlMakeCurrentDelegate = &SDL_GL_MakeCurrent;
        private static readonly delegate* <IntPtr, void> _sdlGlDeleteContextDelegate = &SDL_GL_DeleteContext;
        private static readonly delegate* <IntPtr, IntPtr> _sdlGlGetProcAddressDelegate = &SDL_GL_GetProcAddress;
        private static readonly delegate* <IntPtr, void> _sdlGlSwapWindowDelegate = &SDL_GL_SwapWindow;
        private static readonly delegate* <GlAttribute, int, void> _sdlGlSetAttributeDelegate = &SDL_GL_SetAttribute;
        private static readonly delegate* <GlAttribute, SdlProfile, void> _sdlGlSetAttributeProfileDelegate = &SDL_GL_SetAttributeProfile;
        private static readonly delegate* <GlAttribute, SdlContext, void> _sdlGlSetAttributeContextDelegate = &SDL_GL_SetAttributeContext;
        private static readonly delegate* <int, int> _sdlGlSetSwapInterval = &SDL_GL_SetSwapInterval;

        private static readonly delegate* <IntPtr, IntPtr, uint> _sdlGetGlobalMouseState = &SDL_GetGlobalMouseState;
        private static readonly delegate* <out int, out int, uint> _sdlGetRelativeMouseState = &SDL_GetRelativeMouseState;
        private static readonly delegate* <IntPtr, byte*, void> _sdlSetWindowTitle = &SDL_SetWindowTitle;
        private static readonly delegate* <IntPtr, IntPtr> _sdlGetWindowTitle = &SDL_GetWindowTitle;
        private static readonly delegate* <SdlBool> _sdlGetRelativeMouseMode = &SDL_GetRelativeMouseMode;
        private static readonly delegate* <IntPtr, out int, out int, void> _sdlGetWindowPosition = &SDL_GetWindowPosition;
        private static readonly delegate* <out int, out int, uint> _sdlGetMouseState = &SDL_GetMouseState;
        private static readonly delegate* <IntPtr, int, int, void> _sdlWarpMouseInWindow = &SDL_WarpMouseInWindow;
        private static readonly delegate* <out int, IntPtr> _sdlGetKeyboardState = &SDL_GetKeyboardState;
        private static readonly delegate* <KeyCode, Scancode> _sdlScanCodeFromKeycode = &SDL_GetScancodeFromKey;
        private static readonly delegate* <void> _sdlPumpEvents = &SDL_PumpEvents;
        private static readonly delegate* <IntPtr, out int, out int, void> _sdlGetWindowSize = &SDL_GetWindowSize;

        [DllImport(SdlLibrary)]
        private static extern int SDL_Init(InitFlags initFlags);

        [DllImport(SdlLibrary)]
        private static extern void SDL_Quit();
        
        [DllImport(SdlLibrary)]
        private static extern int SDL_GetDisplayBounds(int displayIndex, out Rectangle rect);

        [DllImport(SdlLibrary)]
        private static extern IntPtr SDL_CreateWindow(IntPtr windowTitle, int x, int y, int width, int height, WindowFlags flags);

        [DllImport(SdlLibrary)]
        private static extern void SDL_DestroyWindow(IntPtr windowHandle);

        [DllImport(SdlLibrary)]
        private static extern IntPtr SDL_GL_CreateContext(IntPtr windowHandle);

        [DllImport(SdlLibrary)]
        private static extern void SDL_GL_MakeCurrent(IntPtr windowHandle, IntPtr renderContext);

        [DllImport(SdlLibrary)]
        private static extern void SDL_GL_DeleteContext(IntPtr renderContext);

        [DllImport(SdlLibrary)]
        private static extern IntPtr SDL_GL_GetProcAddress(IntPtr procName);

        [DllImport(SdlLibrary)]
        private static extern void SDL_GL_SwapWindow(IntPtr windowHandle);

        [DllImport(SdlLibrary, EntryPoint = "SDL_GL_SetAttribute")]
        private static extern void SDL_GL_SetAttribute(GlAttribute attribute, int value);

        [DllImport(SdlLibrary, EntryPoint = "SDL_GL_SetAttribute")]
        private static extern void SDL_GL_SetAttributeProfile(GlAttribute attribute, SdlProfile profile);

        [DllImport(SdlLibrary, EntryPoint = "SDL_GL_SetAttribute")]
        private static extern void SDL_GL_SetAttributeContext(GlAttribute attribute, SdlContext context);

        [DllImport(SdlLibrary)]
        private static extern int SDL_PollEvent(SdlEvent* ev);

        [DllImport(SdlLibrary)]
        private static extern void SDL_PumpEvents();

        [DllImport(SdlLibrary)]
        private static extern uint SDL_GetGlobalMouseState(IntPtr x, IntPtr y);

        [DllImport(SdlLibrary)]
        private static extern uint SDL_GetRelativeMouseState(out int x, out int y);

        [DllImport(SdlLibrary)]
        private static extern void SDL_SetWindowTitle(IntPtr windowHandle, byte* windowTitle);

        [DllImport(SdlLibrary)]
        private static extern IntPtr SDL_GetWindowTitle(IntPtr windowHandle);

        [DllImport(SdlLibrary)]
        private static extern SdlBool SDL_GetRelativeMouseMode();

        [DllImport(SdlLibrary)]
        private static extern void SDL_GetWindowPosition(IntPtr windowHandle, out int x, out int y);

        [DllImport(SdlLibrary)]
        private static extern uint SDL_GetMouseState(out int x, out int y);

        [DllImport(SdlLibrary)]
        private static extern void SDL_WarpMouseInWindow(IntPtr windowHandle, int x, int y);

        [DllImport(SdlLibrary)]
        private static extern IntPtr SDL_GetKeyboardState(out int numberOfKeys);

        [DllImport(SdlLibrary)]
        private static extern Scancode SDL_GetScancodeFromKey(KeyCode key);

        [DllImport(SdlLibrary)]
        private static extern int SDL_GL_SetSwapInterval(int interval);

        [DllImport(SdlLibrary)]
        private static extern void SDL_GetWindowSize(IntPtr windowHandle, out int width, out int height);
    }
}
