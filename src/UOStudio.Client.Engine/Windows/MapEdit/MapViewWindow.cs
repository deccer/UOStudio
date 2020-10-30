using System;
using System.Numerics;
using ImGuiNET;

namespace UOStudio.Client.Engine.Windows.MapEdit
{
    public class MapViewWindow : Window
    {
        private readonly MapEditState _mapEditState;
        private readonly IntPtr _mapViewTextureId;

        public MapViewWindow(MapEditState mapEditState, IntPtr mapViewTextureId)
            : base("Map")
        {
            _mapEditState = mapEditState;
            _mapViewTextureId = mapViewTextureId;
            Show();
        }

        protected override void DrawInternal()
        {
            var padding = ImGui.GetStyle().WindowPadding;
            var itemSpacing = ImGui.GetStyle().ItemSpacing;
            var itemInnerSpacing = ImGui.GetStyle().ItemInnerSpacing;
            ImGui.SetCursorPos(-(padding + itemSpacing + itemInnerSpacing));
            ImGui.Image(_mapViewTextureId, new Vector2(400, 300));
        }
    }
}
