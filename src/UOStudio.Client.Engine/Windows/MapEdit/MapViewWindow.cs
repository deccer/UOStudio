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
            ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, new Vector2(0, 0));
            ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, new Vector2(0, 0));
            ImGui.Image(_mapViewTextureId, new Vector2(400, 300));
            ImGui.PopStyleVar();
        }
    }
}
