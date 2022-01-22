﻿using UOStudio.Client.Engine.Native;

namespace UOStudio.Client.Engine.Input
{
    public static class Keyboard
    {
        internal static readonly byte[] _keyState = new byte[512];
        internal static readonly byte[] _previousKeyState = new byte[512];

        private static readonly Dictionary<int, Sdl.Scancode> _keysToScanCode = new()
        {
            { (int)Keys.A, Sdl.Scancode.A },
            { (int)Keys.B, Sdl.Scancode.B },
            { (int)Keys.C, Sdl.Scancode.C },
            { (int)Keys.D, Sdl.Scancode.D },
            { (int)Keys.E, Sdl.Scancode.E },
            { (int)Keys.F, Sdl.Scancode.F },
            { (int)Keys.G, Sdl.Scancode.G },
            { (int)Keys.H, Sdl.Scancode.H },
            { (int)Keys.I, Sdl.Scancode.I },
            { (int)Keys.J, Sdl.Scancode.J },
            { (int)Keys.K, Sdl.Scancode.K },
            { (int)Keys.L, Sdl.Scancode.L },
            { (int)Keys.M, Sdl.Scancode.M },
            { (int)Keys.N, Sdl.Scancode.N },
            { (int)Keys.O, Sdl.Scancode.O },
            { (int)Keys.P, Sdl.Scancode.P },
            { (int)Keys.Q, Sdl.Scancode.Q },
            { (int)Keys.R, Sdl.Scancode.R },
            { (int)Keys.S, Sdl.Scancode.S },
            { (int)Keys.T, Sdl.Scancode.T },
            { (int)Keys.U, Sdl.Scancode.U },
            { (int)Keys.V, Sdl.Scancode.V },
            { (int)Keys.W, Sdl.Scancode.W },
            { (int)Keys.X, Sdl.Scancode.X },
            { (int)Keys.Y, Sdl.Scancode.Y },
            { (int)Keys.Z, Sdl.Scancode.Z },
            { (int)Keys.D0, Sdl.Scancode.Zero },
            { (int)Keys.D1, Sdl.Scancode.One },
            { (int)Keys.D2, Sdl.Scancode.Two },
            { (int)Keys.D3, Sdl.Scancode.Three },
            { (int)Keys.D4, Sdl.Scancode.Four },
            { (int)Keys.D5, Sdl.Scancode.Five },
            { (int)Keys.D6, Sdl.Scancode.Six },
            { (int)Keys.D7, Sdl.Scancode.Seven },
            { (int)Keys.D8, Sdl.Scancode.Eight },
            { (int)Keys.D9, Sdl.Scancode.Nine },
            { (int)Keys.NumPad0, Sdl.Scancode.KeypadZero },
            { (int)Keys.NumPad1, Sdl.Scancode.KeypadOne },
            { (int)Keys.NumPad2, Sdl.Scancode.KeypadTwo },
            { (int)Keys.NumPad3, Sdl.Scancode.KeypadThree },
            { (int)Keys.NumPad4, Sdl.Scancode.KeypadFour },
            { (int)Keys.NumPad5, Sdl.Scancode.KeypadFive },
            { (int)Keys.NumPad6, Sdl.Scancode.KeypadSix },
            { (int)Keys.NumPad7, Sdl.Scancode.KeypadSeven },
            { (int)Keys.NumPad8, Sdl.Scancode.KeypadEight },
            { (int)Keys.NumPad9, Sdl.Scancode.KeypadNine },
            { (int)Keys.OemClear, Sdl.Scancode.KeypadClear },
            { (int)Keys.Decimal, Sdl.Scancode.KeypadDecimal },
            { (int)Keys.Divide, Sdl.Scancode.KeypadDivide },
            { (int)Keys.Multiply, Sdl.Scancode.KeypadMultiply },
            { (int)Keys.Subtract, Sdl.Scancode.KeypadMinus },
            { (int)Keys.Add, Sdl.Scancode.KeypadPlus },
            { (int)Keys.F1, Sdl.Scancode.F1 },
            { (int)Keys.F2, Sdl.Scancode.F2 },
            { (int)Keys.F3, Sdl.Scancode.F3 },
            { (int)Keys.F4, Sdl.Scancode.F4 },
            { (int)Keys.F5, Sdl.Scancode.F5 },
            { (int)Keys.F6, Sdl.Scancode.F6 },
            { (int)Keys.F7, Sdl.Scancode.F7 },
            { (int)Keys.F8, Sdl.Scancode.F8 },
            { (int)Keys.F9, Sdl.Scancode.F9 },
            { (int)Keys.F10, Sdl.Scancode.F10 },
            { (int)Keys.F11, Sdl.Scancode.F11 },
            { (int)Keys.F12, Sdl.Scancode.F12 },
            { (int)Keys.F13, Sdl.Scancode.F13 },
            { (int)Keys.F14, Sdl.Scancode.F14 },
            { (int)Keys.F15, Sdl.Scancode.F15 },
            { (int)Keys.F16, Sdl.Scancode.F16 },
            { (int)Keys.F17, Sdl.Scancode.F17 },
            { (int)Keys.F18, Sdl.Scancode.F18 },
            { (int)Keys.F19, Sdl.Scancode.F19 },
            { (int)Keys.F20, Sdl.Scancode.F20 },
            { (int)Keys.F21, Sdl.Scancode.F21 },
            { (int)Keys.F22, Sdl.Scancode.F22 },
            { (int)Keys.F23, Sdl.Scancode.F23 },
            { (int)Keys.F24, Sdl.Scancode.F24 },
            { (int)Keys.Space, Sdl.Scancode.Space },
            { (int)Keys.Up, Sdl.Scancode.Up },
            { (int)Keys.Down, Sdl.Scancode.Down },
            { (int)Keys.Left, Sdl.Scancode.Left },
            { (int)Keys.Right, Sdl.Scancode.Right },
            { (int)Keys.LeftAlt, Sdl.Scancode.LeftAlt },
            { (int)Keys.RightAlt, Sdl.Scancode.RightAlt },
            { (int)Keys.LeftControl, Sdl.Scancode.LeftControl },
            { (int)Keys.RightControl, Sdl.Scancode.RightControl },
            { (int)Keys.LeftWindows, Sdl.Scancode.LeftMeta },
            { (int)Keys.RightWindows, Sdl.Scancode.RightMeta },
            { (int)Keys.LeftShift, Sdl.Scancode.LeftShift },
            { (int)Keys.RightShift, Sdl.Scancode.RightShift },
            { (int)Keys.Apps, Sdl.Scancode.Application },
            { (int)Keys.OemQuestion, Sdl.Scancode.Slash },
            { (int)Keys.OemPipe, Sdl.Scancode.BackSlash },
            { (int)Keys.OemOpenBrackets, Sdl.Scancode.LeftBracket },
            { (int)Keys.OemCloseBrackets, Sdl.Scancode.RightBracket },
            { (int)Keys.CapsLock, Sdl.Scancode.CapsLock },
            { (int)Keys.OemComma, Sdl.Scancode.Comma },
            { (int)Keys.Delete, Sdl.Scancode.Delete },
            { (int)Keys.End, Sdl.Scancode.End },
            { (int)Keys.Back, Sdl.Scancode.Backspace },
            { (int)Keys.Enter, Sdl.Scancode.Return },
            { (int)Keys.Escape, Sdl.Scancode.Escape },
            { (int)Keys.Home, Sdl.Scancode.Home },
            { (int)Keys.Insert, Sdl.Scancode.Insert },
            { (int)Keys.OemMinus, Sdl.Scancode.Minus },
            { (int)Keys.NumLock, Sdl.Scancode.NumLockClear },
            { (int)Keys.PageUp, Sdl.Scancode.PageUp },
            { (int)Keys.PageDown, Sdl.Scancode.PageDown },
            { (int)Keys.Pause, Sdl.Scancode.Pause },
            { (int)Keys.OemPeriod, Sdl.Scancode.Period },
            { (int)Keys.OemPlus, Sdl.Scancode.KeypadEqualSign },
            { (int)Keys.PrintScreen, Sdl.Scancode.PrintScreen },
            { (int)Keys.OemQuotes, Sdl.Scancode.Apostrophe },
            { (int)Keys.Scroll, Sdl.Scancode.ScrollLock },
            { (int)Keys.OemSemicolon, Sdl.Scancode.Semicolon },
            { (int)Keys.Sleep, Sdl.Scancode.Sleep },
            { (int)Keys.Tab, Sdl.Scancode.Tab },
            { (int)Keys.OemTilde, Sdl.Scancode.Grave },
            { (int)Keys.VolumeUp, Sdl.Scancode.VolumeUp },
            { (int)Keys.VolumeDown, Sdl.Scancode.VolumeDown },
            { (int)Keys.None, Sdl.Scancode.Unknown }
        };

        public static bool GetKey(Keys keys)
        {
            return _keysToScanCode.TryGetValue((int)keys, out var scanCode) && _keyState[(int)scanCode] == 1;
        }
    }
}