using System;

namespace UOStudio.Client.Engine.Native
{
    public static partial class Sdl
    {
        [Flags]
        public enum InitFlags
        {
            Timer = 0x00000001,
            Audio = 0x00000010,
            Video = 0x00000020,
            Joystick = 0x00000200,
            Haptic = 0x00001000,
            GameController = 0x00002000,
            Events = 0x00004000,
            Sensor = 0x00008000,
            NoParachute = 0x00100000,
            Everything = Timer | Audio | Video |
                         Events | Joystick | Haptic |
                         GameController | Sensor
        }

        [Flags]
        public enum WindowFlags
        {
            Fullscreen = 0x00000001,
            SupportOpenGL = 0x00000002,
            Show = 0x00000004,
            Hidden = 0x00000008,
            Borderless = 0x00000010,
            Resizable = 0x00000020,
            Minimized = 0x00000040,
            Maximized = 0x00000080,
            InputGrabbed = 0x00000100,
            InputFocus = 0x00000200,
            MouseFocus = 0x00000400,
            FullscreenDesktop = (Fullscreen | 0x00001000),
            Foreign = 0x00000800,
            AllowHighDpi = 0x00002000,
            MouseCapture = 0x00004000,
            AlwaysOnTop = 0x00008000,
            SkipTaskbar = 0x00010000,
            Utility = 0x00020000,
            ToolTip = 0x00040000,
            PopupMenu = 0x00080000,
            SupportVulkan = 0x10000000,
            SupportMetal = 0x2000000,
        }

        public enum GlAttribute
        {
            RedSize,
            GreenSize,
            BlueSize,
            AlphaSize,
            BufferSize,
            DoubleBuffer,
            DepthSize,
            StencilSize,
            AccumulatorRedSize,
            AccumulatorGreenSize,
            AccumulatorBlueSize,
            AccumulatorAlphaSize,
            Stereo,
            MultisampleBuffers,
            MultisampleSamples,
            AcceleratedVisual,
            RetainedBacking,
            ContextMajorVersion,
            ContextMinorVersion,
            ContextEgl,
            ContextFlags,
            Profile,
            ShareWithCurrentContext,
            SrgbCapable,
            ReleaseBehaviour,
            ResetNotification,
            ContextNoError
        }

        /*********************************************************************************************/

        public enum DisplayEventId : byte
        {
            None,
            Orientation,
            Connected, /* Requires >= 2.0.14 */
            Disconnected /* Requires >= 2.0.14 */
        }

        public enum EventType : uint
        {
            FirstEvent = 0,

            /* Application events */
            Quit = 0x100,

            /* iOS/Android/WinRT app events */
            ApplicationTerminating,
            ApplicationLowMemory,
            ApplicationWillEnterBackground,
            ApplicationDidEnterBackground,
            ApplicationWillEnterForeground,
            ApplicationDidEnterForeground,

            /* Only available in SDL 2.0.14 or higher. */
            LocaleChanged,

            /* Display events */
            /* Only available in SDL 2.0.9 or higher. */
            DisplayEvent = 0x150,

            /* Window events */
            WindowEvent = 0x200,
            SystemWindowManagerEvent,

            /* Keyboard events */
            KeyDown = 0x300,
            KeyUp,
            TextEditing,
            TextInput,
            KeyMapChanged,

            /* Mouse events */
            MouseMotion = 0x400,
            MouseButtonDown,
            MouseButtonUp,
            Mousewheel,

            /* Joystick events */
            JoystickAxisMotion = 0x600,
            JoystickBallMotion,
            JoystickHatMotion,
            JoystickButtonDown,
            JoystickButtonUp,
            JoystickAdded,
            JoystickRemoved,

            /* Game controller events */
            ControllerAxisMotion = 0x650,
            ControllerButtonDown,
            ControllerButtonUp,
            ControllerDeviceAdded,
            ControllerDeviceRemoved,
            ControllerDeviceRemapped,
            ControllerTouchpadDown, /* Requires >= 2.0.14 */
            ControllerTouchpadMotion, /* Requires >= 2.0.14 */
            ControllerTouchpadUp, /* Requires >= 2.0.14 */
            ControllerSensorUpdate, /* Requires >= 2.0.14 */

            /* Touch events */
            FingerDown = 0x700,
            FingerUp,
            FingerMotion,

            /* Gesture events */
            DollarGesture = 0x800,
            DollarRecord,
            MultiGesture,

            /* Clipboard events */
            ClipboardUpdate = 0x900,

            /* Drag and drop events */
            DropFile = 0x1000,

            /* Only available in 2.0.4 or higher. */
            DropText,
            DropBegin,
            DropComplete,

            /* Audio hotplug events */
            /* Only available in SDL 2.0.4 or higher. */
            AudioDeviceAdded = 0x1100,
            AudioDeviceRemoved,

            /* Sensor events */
            /* Only available in SDL 2.0.9 or higher. */
            SensorUpdate = 0x1200,

            /* Render events */
            /* Only available in SDL 2.0.2 or higher. */
            RenderTargetsReset = 0x2000,

            /* Only available in SDL 2.0.4 or higher. */
            RenderDeviceReset,

            /* Events SDL_USEREVENT through SDL_LASTEVENT are for
             * your use, and should be allocated with
             * SDL_RegisterEvents()
             */
            UserEvent = 0x8000,

            /* The last event, used for bouding arrays. */
            LastEvent = 0xFFFF
        }

        public enum KeyCode
        {
            Unknown = 0,

            Return = '\r',
            Escape = 27, // '\033'
            Backspace = '\b',
            Tab = '\t',
            Space = ' ',
            Exclaim = '!',
            DoubleQuote = '"',
            Hash = '#',
            Percent = '%',
            Dollar = '$',
            Ampersand = '&',
            Quote = '\'',
            LeftParenthesis = '(',
            RightParenthesis = ')',
            Asterisk = '*',
            Plus = '+',
            Comma = ',',
            Minus = '-',
            Period = '.',
            Slash = '/',
            Zero = '0',
            One = '1',
            Two = '2',
            Three = '3',
            Four = '4',
            Five = '5',
            Six = '6',
            Seven = '7',
            Eight = '8',
            Nine = '9',
            Colon = ':',
            Semicolon = ';',
            Less = '<',
            EqualSign = '=',
            Greater = '>',
            Question = '?',
            At = '@',

            /*
            Skip uppercase letters
            */
            LeftBracket = '[',
            BackSlash = '\\',
            RightBracket = ']',
            Caret = '^',
            Underscore = '_',
            BackQuote = '`',
            A = 'a',
            B = 'b',
            C = 'c',
            D = 'd',
            E = 'e',
            F = 'f',
            G = 'g',
            H = 'h',
            I = 'i',
            J = 'j',
            K = 'k',
            L = 'l',
            M = 'm',
            N = 'n',
            O = 'o',
            P = 'p',
            Q = 'q',
            R = 'r',
            S = 's',
            T = 't',
            U = 'u',
            V = 'v',
            W = 'w',
            X = 'x',
            Y = 'y',
            Z = 'z',

            CapsLock = Scancode.CapsLock | ScanCodeMask,

            F1 = Scancode.F1 | ScanCodeMask,
            F2 = Scancode.F2 | ScanCodeMask,
            F3 = Scancode.F3 | ScanCodeMask,
            F4 = Scancode.F4 | ScanCodeMask,
            F5 = Scancode.F5 | ScanCodeMask,
            F6 = Scancode.F6 | ScanCodeMask,
            F7 = Scancode.F7 | ScanCodeMask,
            F8 = Scancode.F8 | ScanCodeMask,
            F9 = Scancode.F9 | ScanCodeMask,
            F10 = Scancode.F10 | ScanCodeMask,
            F11 = Scancode.F11 | ScanCodeMask,
            F12 = Scancode.F12 | ScanCodeMask,

            PrintScreen = Scancode.PrintScreen | ScanCodeMask,
            ScrollLock = Scancode.ScrollLock | ScanCodeMask,
            Pause = Scancode.Pause | ScanCodeMask,
            Insert = Scancode.Insert | ScanCodeMask,
            Home = Scancode.Home | ScanCodeMask,
            PageUp = Scancode.PageUp | ScanCodeMask,
            Delete = 127,
            End = Scancode.End | ScanCodeMask,
            PageDown = Scancode.PageDown | ScanCodeMask,
            Right = Scancode.Right | ScanCodeMask,
            Left = Scancode.Left | ScanCodeMask,
            Down = Scancode.Down | ScanCodeMask,
            Up = Scancode.Up | ScanCodeMask,

            NumLockClear = Scancode.NumLockClear | ScanCodeMask,
            KeypadDivide = Scancode.KeypadDivide | ScanCodeMask,
            KeypadMultiply = Scancode.KeypadMultiply | ScanCodeMask,
            KeypadMinus = Scancode.KeypadMinus | ScanCodeMask,
            KeypadPlus = Scancode.KeypadPlus | ScanCodeMask,
            KeypadEnter = Scancode.KeypadEnter | ScanCodeMask,
            KeypadOne = Scancode.KeypadOne | ScanCodeMask,
            KeypadTwo = Scancode.KeypadTwo | ScanCodeMask,
            KeypadThree = Scancode.KeypadThree | ScanCodeMask,
            KeypadFour = Scancode.KeypadFour | ScanCodeMask,
            KeypadFive = Scancode.KeypadFive | ScanCodeMask,
            KeypadSix = Scancode.KeypadSix | ScanCodeMask,
            KeypadSeven = Scancode.KeypadSeven | ScanCodeMask,
            KeypadEight = Scancode.KeypadEight | ScanCodeMask,
            KeypadNine = Scancode.KeypadNine | ScanCodeMask,
            KeypadZero = Scancode.KeypadZero | ScanCodeMask,
            KeypadPeriod = Scancode.KeypadPeriod | ScanCodeMask,

            Application = Scancode.Application | ScanCodeMask,
            Power = Scancode.Power | ScanCodeMask,
            KeypadEqualSign = Scancode.KeypadEqualSign | ScanCodeMask,
            F13 = Scancode.F13 | ScanCodeMask,
            F14 = Scancode.F14 | ScanCodeMask,
            F15 = Scancode.F15 | ScanCodeMask,
            F16 = Scancode.F16 | ScanCodeMask,
            F17 = Scancode.F17 | ScanCodeMask,
            F18 = Scancode.F18 | ScanCodeMask,
            F19 = Scancode.F19 | ScanCodeMask,
            F20 = Scancode.F20 | ScanCodeMask,
            F21 = Scancode.F21 | ScanCodeMask,
            F22 = Scancode.F22 | ScanCodeMask,
            F23 = Scancode.F23 | ScanCodeMask,
            F24 = Scancode.F24 | ScanCodeMask,
            Execute = Scancode.Execute | ScanCodeMask,
            Help = Scancode.Help | ScanCodeMask,
            Menu = Scancode.Menu | ScanCodeMask,
            Select = Scancode.Select | ScanCodeMask,
            Stop = Scancode.Stop | ScanCodeMask,
            Again = Scancode.Again | ScanCodeMask,
            Undo = Scancode.Undo | ScanCodeMask,
            Cut = Scancode.Cut | ScanCodeMask,
            Copy = Scancode.Copy | ScanCodeMask,
            Paste = Scancode.Paste | ScanCodeMask,
            Find = Scancode.Find | ScanCodeMask,
            Mute = Scancode.Mute | ScanCodeMask,
            VolumeUp = Scancode.VolumeUp | ScanCodeMask,
            VolumeDown = Scancode.VolumeDown | ScanCodeMask,
            KeypadComma = Scancode.KeypadComma | ScanCodeMask,

            KeypadEqualSignAs400 = Scancode.KeypadEqualSignAs400 | ScanCodeMask,

            Alterase = Scancode.Alterase | ScanCodeMask,
            SysReq = Scancode.SysReq | ScanCodeMask,
            Cancel = Scancode.Cancel | ScanCodeMask,
            Clear = Scancode.Clear | ScanCodeMask,
            Prior = Scancode.Prior | ScanCodeMask,
            Return2 = Scancode.Return2 | ScanCodeMask,
            Separator = Scancode.Separator | ScanCodeMask,
            Out = Scancode.Out | ScanCodeMask,
            Oper = Scancode.Oper | ScanCodeMask,
            ClearAgain = Scancode.ClearAgain | ScanCodeMask,
            CrSel = Scancode.CrSel | ScanCodeMask,
            ExSel = Scancode.ExSel | ScanCodeMask,

            KeypadZeroZero = Scancode.KeypadZeroZero | ScanCodeMask,
            KeypadZeroZeroZero = Scancode.KeypadZeroZeroZero | ScanCodeMask,

            ThousandsSeparator = Scancode.ThousandsSeparator | ScanCodeMask,

            DecimalSeparator = Scancode.DecimalSeparator | ScanCodeMask,
            CurrencyUnit = Scancode.CurrencyUnit | ScanCodeMask,

            CurrencySubUnit = Scancode.CurrencySubUnit | ScanCodeMask,
            KeypadLeftParenthesis = Scancode.KeypadLeftParenthesis | ScanCodeMask,
            KeypadRightParenthesis = Scancode.KeypadRightParenthesis | ScanCodeMask,
            KeypadLeftBrace = Scancode.KeypadLeftBrace | ScanCodeMask,
            KeypadRightBrace = Scancode.KeypadRightBrace | ScanCodeMask,
            KeypadTab = Scancode.KeypadTab | ScanCodeMask,
            KeypadBackspace = Scancode.KeypadBackspace | ScanCodeMask,
            KeypadA = Scancode.KeypadA | ScanCodeMask,
            KeypadB = Scancode.KeypadB | ScanCodeMask,
            KeypadC = Scancode.KeypadC | ScanCodeMask,
            KeypadD = Scancode.KeypadD | ScanCodeMask,
            KeypadE = Scancode.KeypadE | ScanCodeMask,
            KeypadF = Scancode.KeypadF | ScanCodeMask,
            KeypadXor = Scancode.KeypadXor | ScanCodeMask,
            KeypadPower = Scancode.KeypadPower | ScanCodeMask,
            KeypadPercent = Scancode.KeypadPercent | ScanCodeMask,
            KeypadLess = Scancode.KeypadLess | ScanCodeMask,
            KeypadGreater = Scancode.KeypadGreater | ScanCodeMask,
            KeypadAmpersand = Scancode.KeypadAmpersand | ScanCodeMask,
            KeypadDoubleAmpersand = Scancode.KeypadDoubleAmpersand | ScanCodeMask,
            KeypadVerticalBar = Scancode.KeypadVerticalBar | ScanCodeMask,
            KeypadDoubleVerticaclBar = Scancode.KeypadDoubleVerticaclBar | ScanCodeMask,
            KeypadColon = Scancode.KeypadColon | ScanCodeMask,
            KeypadHash = Scancode.KeypadHash | ScanCodeMask,
            KeypadSpace = Scancode.KeypadSpace | ScanCodeMask,
            KeypadAt = Scancode.KeypadAt | ScanCodeMask,
            KeypadExclamation = Scancode.KeypadExclamation | ScanCodeMask,
            KeypadMemoryStore = Scancode.KeypadMemoryStore | ScanCodeMask,
            KeypadMemoryRecall = Scancode.KeypadMemoryRecall | ScanCodeMask,
            KeypadMemoryClear = Scancode.KeypadMemoryClear | ScanCodeMask,
            KeypadMemoryAdd = Scancode.KeypadMemoryAdd | ScanCodeMask,

            KeypadMemorySubtract = Scancode.KeypadMemorySubtract | ScanCodeMask,
            KeypadMemoryMultiply = Scancode.KeypadMemoryMultiply | ScanCodeMask,
            KeypadMemoryDivide = Scancode.KeypadMemoryDivide | ScanCodeMask,
            KeypadPlusMinus = Scancode.KeypadPlusMinus | ScanCodeMask,
            KeypadClear = Scancode.KeypadClear | ScanCodeMask,
            KeypadClearEntry = Scancode.KeypadClearEntry | ScanCodeMask,
            KeypadBinary = Scancode.KeypadBinary | ScanCodeMask,
            KeypadOctal = Scancode.KeypadOctal | ScanCodeMask,
            KeypadDecimal = Scancode.KeypadDecimal | ScanCodeMask,

            KeypadHexadecimal = Scancode.KeypadHexadecimal | ScanCodeMask,

            LeftControl = Scancode.LeftControl | ScanCodeMask,
            LeftShift = Scancode.LeftShift | ScanCodeMask,
            LeftAlt = Scancode.LeftAlt | ScanCodeMask,
            LeftMeta = Scancode.LeftMeta | ScanCodeMask,
            RightControl = Scancode.RightControl | ScanCodeMask,
            RightShift = Scancode.RightShift | ScanCodeMask,
            RightAlt = Scancode.RightAlt | ScanCodeMask,
            RightMeta = Scancode.RightMeta | ScanCodeMask,

            Mode = Scancode.Mode | ScanCodeMask,

            AudioNext = Scancode.AudioNext | ScanCodeMask,
            AudioPrev = Scancode.AudioPrev | ScanCodeMask,
            AudioStop = Scancode.AudioStop | ScanCodeMask,
            AudioPlay = Scancode.AudioPlay | ScanCodeMask,
            AudioMute = Scancode.AudioMute | ScanCodeMask,
            MediaSelect = Scancode.MediaSelect | ScanCodeMask,
            Browser = Scancode.Browser | ScanCodeMask,
            Mail = Scancode.Mail | ScanCodeMask,
            Calculator = Scancode.Calculator | ScanCodeMask,
            Computer = Scancode.Computer | ScanCodeMask,
            ApplicationSearch = Scancode.ApplicationSearch | ScanCodeMask,
            ApplicationHome = Scancode.ApplicationHome | ScanCodeMask,
            ApplicationBack = Scancode.ApplicationBack | ScanCodeMask,
            ApplicationForward = Scancode.ApplicationForward | ScanCodeMask,
            ApplicationStop = Scancode.ApplicationStop | ScanCodeMask,
            ApplicationRefresh = Scancode.ApplicationRefresh | ScanCodeMask,
            ApplicationBookmarks = Scancode.ApplicationBookmarks | ScanCodeMask,

            BrightnessDown = Scancode.BrightnessDown | ScanCodeMask,
            BrightnessUp = Scancode.BrightnessUp | ScanCodeMask,
            DisplaySwitch = Scancode.DisplaySwitch | ScanCodeMask,

            KeyboardIlluminationToggle = Scancode.KeyboardIlluminationToggle | ScanCodeMask,
            KeyboardIlluminationDown = Scancode.KeyboardIlluminationDown | ScanCodeMask,
            KeyboardIlluminationUp = Scancode.KeyboardIlluminationUp | ScanCodeMask,
            Eject = Scancode.Eject | ScanCodeMask,
            Sleep = Scancode.Sleep | ScanCodeMask,
            App1 = Scancode.App1 | ScanCodeMask,
            App2 = Scancode.App2 | ScanCodeMask,

            AudioRewind = Scancode.AudioRewind | ScanCodeMask,
            AudioFastForward = Scancode.AudioFastForward | ScanCodeMask
        }

        /* Key modifiers (bitfield) */
        [Flags]
        public enum KeyModifier : ushort
        {
            None = 0x0000,
            LeftShift = 0x0001,
            RightShift = 0x0002,
            LeftControl = 0x0040,
            RightControl = 0x0080,
            LeftAlt = 0x0100,
            RightAlt = 0x0200,
            LeftMeta = 0x0400,
            RightMeta = 0x0800,
            NumLock = 0x1000,
            CapsLock = 0x2000,
            Mode = 0x4000,
            Reserved = 0x8000,

            /* These are defines in the SDL headers */
            Control = LeftControl | RightControl,
            Shift = LeftShift | RightShift,
            Alt = LeftAlt | RightAlt,
            Meta = LeftMeta | RightMeta
        }

        public enum Scancode
        {
            Unknown = 0,

            A = 4,
            B = 5,
            C = 6,
            D = 7,
            E = 8,
            F = 9,
            G = 10,
            H = 11,
            I = 12,
            J = 13,
            K = 14,
            L = 15,
            M = 16,
            N = 17,
            O = 18,
            P = 19,
            Q = 20,
            R = 21,
            S = 22,
            T = 23,
            U = 24,
            V = 25,
            W = 26,
            X = 27,
            Y = 28,
            Z = 29,

            One = 30,
            Two = 31,
            Three = 32,
            Four = 33,
            Five = 34,
            Six = 35,
            Seven = 36,
            Eight = 37,
            Nine = 38,
            Zero = 39,

            Return = 40,
            Escape = 41,
            Backspace = 42,
            Tab = 43,
            Space = 44,

            Minus = 45,
            EqualSign = 46,
            LeftBracket = 47,
            RightBracket = 48,
            BackSlash = 49,
            NonUsHash = 50,
            Semicolon = 51,
            Apostrophe = 52,
            Grave = 53,
            Comma = 54,
            Period = 55,
            Slash = 56,

            CapsLock = 57,

            F1 = 58,
            F2 = 59,
            F3 = 60,
            F4 = 61,
            F5 = 62,
            F6 = 63,
            F7 = 64,
            F8 = 65,
            F9 = 66,
            F10 = 67,
            F11 = 68,
            F12 = 69,

            PrintScreen = 70,
            ScrollLock = 71,
            Pause = 72,
            Insert = 73,
            Home = 74,
            PageUp = 75,
            Delete = 76,
            End = 77,
            PageDown = 78,
            Right = 79,
            Left = 80,
            Down = 81,
            Up = 82,

            NumLockClear = 83,
            KeypadDivide = 84,
            KeypadMultiply = 85,
            KeypadMinus = 86,
            KeypadPlus = 87,
            KeypadEnter = 88,
            KeypadOne = 89,
            KeypadTwo = 90,
            KeypadThree = 91,
            KeypadFour = 92,
            KeypadFive = 93,
            KeypadSix = 94,
            KeypadSeven = 95,
            KeypadEight = 96,
            KeypadNine = 97,
            KeypadZero = 98,
            KeypadPeriod = 99,

            NonUsBackslash = 100,
            Application = 101,
            Power = 102,
            KeypadEqualSign = 103,
            F13 = 104,
            F14 = 105,
            F15 = 106,
            F16 = 107,
            F17 = 108,
            F18 = 109,
            F19 = 110,
            F20 = 111,
            F21 = 112,
            F22 = 113,
            F23 = 114,
            F24 = 115,
            Execute = 116,
            Help = 117,
            Menu = 118,
            Select = 119,
            Stop = 120,
            Again = 121,
            Undo = 122,
            Cut = 123,
            Copy = 124,
            Paste = 125,
            Find = 126,
            Mute = 127,
            VolumeUp = 128,
            VolumeDown = 129,

            LockingCapsLock = 130,
            LockingNumLock = 131,
            LockingScrollLock = 132,
            KeypadComma = 133,
            KeypadEqualSignAs400 = 134,

            InternationalOne = 135,
            InternationalTwo = 136,
            InternationalThree = 137,
            InternationalFour = 138,
            InternationalFive = 139,
            InternationalSix = 140,
            InternationalSeven = 141,
            InternationalEight = 142,
            InternationalNine = 143,
            LanguageOne = 144,
            LanguageTwo = 145,
            LanguageThree = 146,
            LanguageFour = 147,
            LanguageFive = 148,
            LanguageSix = 149,
            LanguageSeven = 150,
            LanguageEight = 151,
            LanguageNine = 152,

            Alterase = 153,
            SysReq = 154,
            Cancel = 155,
            Clear = 156,
            Prior = 157,
            Return2 = 158,
            Separator = 159,
            Out = 160,
            Oper = 161,
            ClearAgain = 162,
            CrSel = 163,
            ExSel = 164,

            KeypadZeroZero = 176,
            KeypadZeroZeroZero = 177,
            ThousandsSeparator = 178,
            DecimalSeparator = 179,
            CurrencyUnit = 180,
            CurrencySubUnit = 181,
            KeypadLeftParenthesis = 182,
            KeypadRightParenthesis = 183,
            KeypadLeftBrace = 184,
            KeypadRightBrace = 185,
            KeypadTab = 186,
            KeypadBackspace = 187,
            KeypadA = 188,
            KeypadB = 189,
            KeypadC = 190,
            KeypadD = 191,
            KeypadE = 192,
            KeypadF = 193,
            KeypadXor = 194,
            KeypadPower = 195,
            KeypadPercent = 196,
            KeypadLess = 197,
            KeypadGreater = 198,
            KeypadAmpersand = 199,
            KeypadDoubleAmpersand = 200,
            KeypadVerticalBar = 201,
            KeypadDoubleVerticaclBar = 202,
            KeypadColon = 203,
            KeypadHash = 204,
            KeypadSpace = 205,
            KeypadAt = 206,
            KeypadExclamation = 207,
            KeypadMemoryStore = 208,
            KeypadMemoryRecall = 209,
            KeypadMemoryClear = 210,
            KeypadMemoryAdd = 211,
            KeypadMemorySubtract = 212,
            KeypadMemoryMultiply = 213,
            KeypadMemoryDivide = 214,
            KeypadPlusMinus = 215,
            KeypadClear = 216,
            KeypadClearEntry = 217,
            KeypadBinary = 218,
            KeypadOctal = 219,
            KeypadDecimal = 220,
            KeypadHexadecimal = 221,

            LeftControl = 224,
            LeftShift = 225,
            LeftAlt = 226,
            LeftMeta = 227,
            RightControl = 228,
            RightShift = 229,
            RightAlt = 230,
            RightMeta = 231,

            Mode = 257,

            /* These come from the USB consumer page (0x0C) */
            AudioNext = 258,
            AudioPrev = 259,
            AudioStop = 260,
            AudioPlay = 261,
            AudioMute = 262,
            MediaSelect = 263,
            Browser = 264,
            Mail = 265,
            Calculator = 266,
            Computer = 267,
            ApplicationSearch = 268,
            ApplicationHome = 269,
            ApplicationBack = 270,
            ApplicationForward = 271,
            ApplicationStop = 272,
            ApplicationRefresh = 273,
            ApplicationBookmarks = 274,

            /* These come from other sources, and are mostly mac related */
            BrightnessDown = 275,
            BrightnessUp = 276,
            DisplaySwitch = 277,
            KeyboardIlluminationToggle = 278,
            KeyboardIlluminationDown = 279,
            KeyboardIlluminationUp = 280,
            Eject = 281,
            Sleep = 282,

            App1 = 283,
            App2 = 284,

            /* These come from the USB consumer page (0x0C) */
            AudioRewind = 285,
            AudioFastForward = 286,

            /* This is not a key, simply marks the number of scancodes
             * so that you know how big to make your arrays. */
            ScancodeCount = 512
        }

        public enum WindowEventId : byte
        {
            None,
            Shown,
            Hidden,
            Exposed,
            Moved,
            Resized,
            SizeChanged,
            Minimized,
            Maximized,
            Restored,
            Enter,
            Leave,
            FocusGained,
            FocusLost,
            Close,

            /* Only available in 2.0.5 or higher. */
            TakeFocus,
            HitTest
        }

        [Flags]
        public enum SdlContext
        {
            Debug = 0x0001,
            ForwardCompatible = 0x0002,
            RobustAccess = 0x0004,
            ResetIsolation = 0x0008
        }

        [Flags]
        public enum SdlProfile
        {
            Core = 0x0001,
            Compatibility = 0x0002,
            Es = 0x0004
        }

        public enum SdlBool
        {
            False = 0,
            True = 1
        }
    }
}
