using System;
using System.Numerics;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

namespace UOStudio.Client.Engine.Windows.MapEdit
{
    public class MapViewWindow : Window
    {
        private readonly EditorState _editorState;
        private readonly IntPtr _mapViewTextureId;
        private readonly float _width;
        private readonly float _height;

        public MapViewWindow(EditorState editorState, IntPtr mapViewTextureId, float width, float height)
            : base("Map")
        {
            _editorState = editorState;
            _mapViewTextureId = mapViewTextureId;
            _width = width;
            _height = height;
        }

        protected override void DrawInternal()
        {
            var padding = ImGui.GetStyle().WindowPadding;
            var itemSpacing = ImGui.GetStyle().ItemSpacing;
            var itemInnerSpacing = ImGui.GetStyle().ItemInnerSpacing;
            ImGui.SetCursorPos(-(padding + itemSpacing + itemInnerSpacing));
            ImGui.Image(_mapViewTextureId, new Vector2(_width, _height));
        }
    }
}
