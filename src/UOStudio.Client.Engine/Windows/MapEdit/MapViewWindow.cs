using System;
using ImGuiNET;
using Vector2 = System.Numerics.Vector2;

namespace UOStudio.Client.Engine.Windows.MapEdit
{
    public class MapViewWindow : Window
    {
        private readonly EditorState _editorState;
        private IntPtr _mapViewTextureId;
        private Vector2 _windowSize;

        public MapViewWindow(EditorState editorState)
            : base("Map")
        {
            _editorState = editorState;
        }

        public event Action<Vector2> OnWindowResize;

        public Vector2 WindowSize { get; private set; }

        public void UpdateRenderTarget(IntPtr mapViewTextureId, int width, int height)
        {
            _mapViewTextureId = mapViewTextureId;
            _windowSize = new Vector2(width, height);
        }

        protected override void DrawInternal()
        {
            var windowSize = ImGui.GetWindowSize();
            if (!windowSize.Equals(_windowSize))
            {
                var onWindowResize = OnWindowResize;
                onWindowResize?.Invoke(windowSize);
                _windowSize = windowSize;
            }
            var style = ImGui.GetStyle();
            var padding = style.WindowPadding;
            var itemSpacing = style.ItemSpacing;
            var itemInnerSpacing = style.ItemInnerSpacing;
            ImGui.SetCursorPos(-(padding + itemSpacing + itemInnerSpacing));
            ImGui.Image(_mapViewTextureId, WindowSize);
        }

        protected override ImGuiWindowFlags GetWindowFlags()
            => ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse;
    }
}
