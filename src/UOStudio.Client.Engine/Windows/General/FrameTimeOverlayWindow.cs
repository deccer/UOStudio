using System;
using ImGuiNET;

namespace UOStudio.Client.Engine.Windows.General
{
    [Flags]
    public enum Position
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Custom
    }

    public sealed class FrameTimeOverlayWindow : Window
    {
        private Position _position;

        public FrameTimeOverlayWindow(string caption)
            : base(caption)
        {
        }

        protected override ImGuiWindowFlags SetWindowFlags()
        {
            const float DISTANCE = 10.0f;
            if (_position != Position.Custom)
            {
                var viewport = ImGui.GetMainViewport();
                var workAreaPos = viewport.GetWorkPos();
                var workAreaSize = viewport.GetWorkSize();
                var windowPos = new System.Numerics.Vector2(
                    (_position & Position.TopRight) == Position.TopRight
                        ? workAreaPos.X + workAreaSize.X - DISTANCE
                        : workAreaPos.X + DISTANCE,
                    (_position & Position.BottomLeft) == Position.BottomLeft
                        ? workAreaPos.Y + workAreaSize.Y - DISTANCE
                        : workAreaPos.Y + DISTANCE
                );
                var windowPosPivot = new System.Numerics.Vector2(
                    ((_position & Position.TopRight) == Position.TopRight) ? 1.0f : 0.0f,
                    ((_position & Position.BottomLeft) == Position.BottomLeft) ? 1.0f : 0.0f);
                ImGui.SetNextWindowPos(windowPos, ImGuiCond.Always, windowPosPivot);
                ImGui.SetNextWindowViewport(viewport.ID);
            }

            ImGui.SetNextWindowBgAlpha(0.5f);
            var windowFlags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoDocking |
                              ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoSavedSettings |
                              ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav;
            if (_position != Position.Custom)
            {
                windowFlags |= ImGuiWindowFlags.NoMove;
            }

            return windowFlags;
        }

        protected override void DrawInternal()
        {
            var io = ImGui.GetIO();

            ImGui.Text($"Average \n{(int)(1000.0f / ImGui.GetIO().Framerate)} ms/frame ({(int)ImGui.GetIO().Framerate} FPS)");
            ImGui.Separator();
            ImGui.Text(ImGui.IsMousePosValid()
                ? $"Mouse Position: ({io.MousePos.X},{io.MousePos.Y})"
                : "Mouse Position: <invalid>");

            if (ImGui.BeginPopupContextWindow())
            {
                if (ImGui.MenuItem("Custom", null, _position == Position.Custom))
                {
                    _position = Position.Custom;
                }

                if (ImGui.MenuItem("Top-left", null, _position == Position.TopLeft))
                {
                    _position = Position.TopLeft;
                }

                if (ImGui.MenuItem("Top-right", null, _position == Position.TopRight))
                {
                    _position = Position.TopRight;
                }

                if (ImGui.MenuItem("Bottom-left", null, _position == Position.BottomLeft))
                {
                    _position = Position.BottomLeft;
                }

                if (ImGui.MenuItem("Bottom-right", null, _position == Position.BottomRight))
                {
                    _position = Position.BottomRight;
                }

                if (IsVisible && ImGui.MenuItem("Close"))
                {
                    Hide();
                }

                ImGui.EndPopup();
            }
        }
    }
}
