using System;
using System.Runtime.InteropServices;

namespace UOStudio.Client.Engine.Native
{
    public static unsafe partial class Sdl
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Rectangle
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KeySymbol
        {
            public Scancode Scancode;
            public KeyCode KeyCode;
            public KeyModifier Modifier; /* UInt16 */
            public uint Unicode; /* Deprecated */
        }

        /* Fields shared by every event */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlGenericEvent
        {
            public EventType Type;
            public uint Timestamp;
        }

// Ignore private members used for padding in this struct
#pragma warning disable 0169
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlDisplayEvent
        {
            public EventType Type;
            public uint Timestamp;
            public uint Display;
            public DisplayEventId DisplayEvent; // event, lolC#
            private readonly byte padding1;
            private readonly byte padding2;
            private readonly byte padding3;
            public int data1;
        }
#pragma warning restore 0169

// Ignore private members used for padding in this struct
#pragma warning disable 0169
        /* Window state change event data (event.window.*) */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlWindowEvent
        {
            public EventType Type;
            public uint Timestamp;
            public uint WindowId;
            public WindowEventId WindowEvent; // event, lolC#
            private readonly byte padding1;
            private readonly byte padding2;
            private readonly byte padding3;
            public int data1;
            public int data2;
        }
#pragma warning restore 0169

// Ignore private members used for padding in this struct
#pragma warning disable 0169
        /* Keyboard button event structure (event.key.*) */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlKeyboardEvent
        {
            public EventType Type;
            public uint Timestamp;
            public uint WindowId;
            public byte State;
            public byte Repeat; /* non-zero if this is a repeat */
            private readonly byte padding2;
            private readonly byte padding3;
            public KeySymbol KeySymbol;
        }
#pragma warning restore 0169

        [StructLayout(LayoutKind.Sequential)]
        public struct SdlTextEditingEvent
        {
            public EventType Type;
            public uint Timestamp;
            public uint WindowId;
            public fixed byte Text[TexteditingEventTextSize];
            public int Start;
            public int Length;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SdlTextInputEvent
        {
            public EventType Type;
            public uint Timestamp;
            public uint WindowId;
            public fixed byte Text[TextInputeventTextSize];
        }

// Ignore private members used for padding in this struct
#pragma warning disable 0169
        /* Mouse motion event structure (event.motion.*) */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlMouseMotionEvent
        {
            public EventType Type;
            public uint Timestamp;
            public uint WindowId;
            public uint Which;
            public byte State; /* bitmask of buttons */
            private readonly byte padding1;
            private readonly byte padding2;
            private readonly byte padding3;
            public int X;
            public int Y;
            public int XRel;
            public int YRel;
        }
#pragma warning restore 0169

// Ignore private members used for padding in this struct
#pragma warning disable 0169
        /* Mouse button event structure (event.button.*) */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlMouseButtonEvent
        {
            public EventType Type;
            public uint Timestamp;
            public uint WindowId;
            public uint Which;
            public byte Button; /* button id */
            public byte State; /* SDL_PRESSED or SDL_RELEASED */
            public byte Clicks; /* 1 for single-click, 2 for double-click, etc. */
            private readonly byte padding1;
            public int X;
            public int Y;
        }
#pragma warning restore 0169

        /* Mouse wheel event structure (event.wheel.*) */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlMouseWheelEvent
        {
            public EventType Type;
            public uint Timestamp;
            public uint WindowId;
            public uint Which;
            public int X; /* amount scrolled horizontally */
            public int Y; /* amount scrolled vertically */
            public uint Direction; /* Set to one of the SDL_MOUSEWHEEL_* defines */
        }

// Ignore private members used for padding in this struct
#pragma warning disable 0169
        /* Joystick axis motion event structure (event.jaxis.*) */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlJoyAxisEvent
        {
            public EventType type;
            public uint timestamp;
            public int which; /* SDL_JoystickID */
            public byte axis;
            private readonly byte padding1;
            private readonly byte padding2;
            private readonly byte padding3;
            public short axisValue; /* value, lolC# */
            public ushort padding4;
        }
#pragma warning restore 0169

// Ignore private members used for padding in this struct
#pragma warning disable 0169
        /* Joystick trackball motion event structure (event.jball.*) */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlJoyBallEvent
        {
            public EventType type;
            public uint timestamp;
            public int which; /* SDL_JoystickID */
            public byte ball;
            private readonly byte padding1;
            private readonly byte padding2;
            private readonly byte padding3;
            public short xrel;
            public short yrel;
        }
#pragma warning restore 0169

// Ignore private members used for padding in this struct
#pragma warning disable 0169
        /* Joystick hat position change event struct (event.jhat.*) */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlJoyHatEvent
        {
            public EventType type;
            public uint timestamp;
            public int which; /* SDL_JoystickID */
            public byte hat; /* index of the hat */
            public byte hatValue; /* value, lolC# */
            private readonly byte padding1;
            private readonly byte padding2;
        }
#pragma warning restore 0169

// Ignore private members used for padding in this struct
#pragma warning disable 0169
        /* Joystick button event structure (event.jbutton.*) */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlJoyButtonEvent
        {
            public EventType type;
            public uint timestamp;
            public int which; /* SDL_JoystickID */
            public byte button;
            public byte state; /* SDL_PRESSED or SDL_RELEASED */
            private readonly byte padding1;
            private readonly byte padding2;
        }
#pragma warning restore 0169

        /* Joystick device event structure (event.jdevice.*) */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlJoyDeviceEvent
        {
            public EventType type;
            public uint timestamp;
            public int which; /* SDL_JoystickID */
        }

// Ignore private members used for padding in this struct
#pragma warning disable 0169
        /* Game controller axis motion event (event.caxis.*) */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlControllerAxisEvent
        {
            public EventType type;
            public uint timestamp;
            public int which; /* SDL_JoystickID */
            public byte axis;
            private readonly byte padding1;
            private readonly byte padding2;
            private readonly byte padding3;
            public short axisValue; /* value, lolC# */
            private readonly ushort padding4;
        }
#pragma warning restore 0169

// Ignore private members used for padding in this struct
#pragma warning disable 0169
        /* Game controller button event (event.cbutton.*) */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlControllerButtonEvent
        {
            public EventType type;
            public uint timestamp;
            public int which; /* SDL_JoystickID */
            public byte button;
            public byte state;
            private readonly byte padding1;
            private readonly byte padding2;
        }
#pragma warning restore 0169

        /* Game controller device event (event.cdevice.*) */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlControllerDeviceEvent
        {
            public EventType type;
            public uint timestamp;
            public int which;
        }

        /* Game controller touchpad event structure (event.ctouchpad.*) */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlControllerTouchpadEvent
        {
            public uint type;
            public uint timestamp;
            public int which; /* SDL_JoystickID */
            public int touchpad;
            public int finger;
            public float x;
            public float y;
            public float pressure;
        }

        /* Game controller sensor event structure (event.csensor.*) */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlControllerSensorEvent
        {
            public uint type;
            public uint timestamp;
            public int which; /* SDL_JoystickID */
            public int sensor;
            public float data1;
            public float data2;
            public float data3;
        }

// Ignore private members used for padding in this struct
#pragma warning disable 0169
        /* Audio device event (event.adevice.*) */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlAudioDeviceEvent
        {
            public uint type;
            public uint timestamp;
            public uint which;
            public byte iscapture;
            private readonly byte padding1;
            private readonly byte padding2;
            private readonly byte padding3;
        }
#pragma warning restore 0169

        [StructLayout(LayoutKind.Sequential)]
        public struct SdlTouchFingerEvent
        {
            public uint type;
            public uint timestamp;
            public long touchId; // SDL_TouchID
            public long fingerId; // SDL_GestureID
            public float x;
            public float y;
            public float dx;
            public float dy;
            public float pressure;
            public uint windowID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SdlMultiGestureEvent
        {
            public uint type;
            public uint timestamp;
            public long touchId; // SDL_TouchID
            public float dTheta;
            public float dDist;
            public float x;
            public float y;
            public ushort numFingers;
            public ushort padding;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SdlDollarGestureEvent
        {
            public uint type;
            public uint timestamp;
            public long touchId; // SDL_TouchID
            public long gestureId; // SDL_GestureID
            public uint numFingers;
            public float error;
            public float x;
            public float y;
        }

        /* File open request by system (event.drop.*), enabled by
         * default
         */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlDropEvent
        {
            public EventType type;
            public uint timestamp;
            public IntPtr file;
            public uint windowID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SdlSensorEvent
        {
            public EventType type;
            public uint timestamp;
            public int which;
            public fixed float data[6];
        }

        /* The "quit requested" event */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlQuitEvent
        {
            public EventType Type;
            public uint Timestamp;
        }

        /* A user defined event (event.user.*) */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlUserEvent
        {
            public uint type;
            public uint timestamp;
            public uint windowID;
            public int code;
            public IntPtr data1; /* user-defined */
            public IntPtr data2; /* user-defined */
        }

        /* A video driver dependent event (event.syswm.*), disabled */
        [StructLayout(LayoutKind.Sequential)]
        public struct SdlSysWmEvent
        {
            public EventType type;
            public uint timestamp;
            public IntPtr msg; /* SDL_SysWMmsg*, system-dependent*/
        }

        /* General event structure */
        // C# doesn't do unions, so we do this ugly thing. */
        [StructLayout(LayoutKind.Explicit)]
        public struct SdlEvent
        {
            [FieldOffset(0)]
            public EventType Type;
            [FieldOffset(0)]
            public EventType TypeFSharp;
            [FieldOffset(0)]
            public SdlDisplayEvent Display;
            [FieldOffset(0)]
            public SdlWindowEvent Window;
            [FieldOffset(0)]
            public SdlKeyboardEvent Key;
            [FieldOffset(0)]
            public SdlTextEditingEvent Edit;
            [FieldOffset(0)]
            public SdlTextInputEvent Text;
            [FieldOffset(0)]
            public SdlMouseMotionEvent Motion;
            [FieldOffset(0)]
            public SdlMouseButtonEvent Button;
            [FieldOffset(0)]
            public SdlMouseWheelEvent Wheel;
            [FieldOffset(0)]
            public SdlJoyAxisEvent JAxis;
            [FieldOffset(0)]
            public SdlJoyBallEvent JBall;
            [FieldOffset(0)]
            public SdlJoyHatEvent JHat;
            [FieldOffset(0)]
            public SdlJoyButtonEvent JButton;
            [FieldOffset(0)]
            public SdlJoyDeviceEvent JDevice;
            [FieldOffset(0)]
            public SdlControllerAxisEvent ControllerAxis;
            [FieldOffset(0)]
            public SdlControllerButtonEvent ControllerButton;
            [FieldOffset(0)]
            public SdlControllerDeviceEvent ControllerDevice;
            [FieldOffset(0)]
            public SdlControllerDeviceEvent ControllerTouchPad;
            [FieldOffset(0)]
            public SdlControllerDeviceEvent ControllerSensor;
            [FieldOffset(0)]
            public SdlAudioDeviceEvent AudioDevice;
            [FieldOffset(0)]
            public SdlSensorEvent Sensor;
            [FieldOffset(0)]
            public SdlQuitEvent Quit;
            [FieldOffset(0)]
            public SdlUserEvent User;
            [FieldOffset(0)]
            public SdlSysWmEvent SystemWindowManager;
            [FieldOffset(0)]
            public SdlTouchFingerEvent TouchFinger;
            [FieldOffset(0)]
            public SdlMultiGestureEvent MultiGesture;
            [FieldOffset(0)]
            public SdlDollarGestureEvent DollarGesture;
            [FieldOffset(0)]
            public SdlDropEvent Drop;
            [FieldOffset(0)]
            private fixed byte padding[56];
        }
    }
}
