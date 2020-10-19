using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ned.Client.Engine.UI
{
    public class ImGuiInputHandler
    {
        public int Scrollwheel { get; set; }

        public List<int> KeyMap { get; }

        public void Update(GraphicsDevice device, ref KeyboardState keyboardState, ref MouseState mouseState)
        {
            var io = ImGui.GetIO();

            for (int i = 0; i < KeyMap.Count; i++)
            {
                io.KeysDown[KeyMap[i]] = keyboardState.IsKeyDown((Keys)KeyMap[i]);
            }

            io.KeyShift = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);
            io.KeyCtrl = keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl);
            io.KeyAlt = keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt);
            io.KeySuper = keyboardState.IsKeyDown(Keys.LeftWindows) || keyboardState.IsKeyDown(Keys.RightWindows);

            io.DisplaySize = new System.Numerics.Vector2(
                device.PresentationParameters.BackBufferWidth,
                device.PresentationParameters.BackBufferHeight
            );
            io.DisplayFramebufferScale = new System.Numerics.Vector2(1f, 1f);

            io.MousePos = new System.Numerics.Vector2(mouseState.X, mouseState.Y);

            io.MouseDown[0] = mouseState.LeftButton == ButtonState.Pressed;
            io.MouseDown[1] = mouseState.RightButton == ButtonState.Pressed;
            io.MouseDown[2] = mouseState.MiddleButton == ButtonState.Pressed;

            var scrollDelta = mouseState.ScrollWheelValue - Scrollwheel;
            io.MouseWheel = scrollDelta > 0
                ? 1
                : scrollDelta < 0
                    ? -1
                    : 0;
            Scrollwheel = mouseState.ScrollWheelValue;
        }

        public ImGuiInputHandler Initialize(Game game)
        {
            var io = ImGui.GetIO();

            KeyMap.Add(io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab);
            KeyMap.Add(io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left);
            KeyMap.Add(io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right);
            KeyMap.Add(io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up);
            KeyMap.Add(io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down);
            KeyMap.Add(io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp);
            KeyMap.Add(io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown);
            KeyMap.Add(io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home);
            KeyMap.Add(io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End);
            KeyMap.Add(io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete);
            KeyMap.Add(io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Back);
            KeyMap.Add(io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter);
            KeyMap.Add(io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape);
            KeyMap.Add(io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A);
            KeyMap.Add(io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C);
            KeyMap.Add(io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V);
            KeyMap.Add(io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X);
            KeyMap.Add(io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y);
            KeyMap.Add(io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z);

            TextInputEXT.TextInput += c =>
            {
                if (c == '\t')
                {
                    return;
                }

                ImGui.GetIO().AddInputCharacter(c);
            };

            io.Fonts.AddFontDefault();
            return this;
        }

        public ImGuiInputHandler()
        {
            Scrollwheel = 0;
            KeyMap = new List<int>();
        }
    }
}
