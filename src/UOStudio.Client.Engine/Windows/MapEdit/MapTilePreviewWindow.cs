using System;
using System.Numerics;
using ImGuiNET;

namespace UOStudio.Client.Engine.Windows.MapEdit
{
    public class MapTilePreviewWindow : Window
    {
        private bool _stretchTexture;

        public MapTilePreviewWindow()
            : base("Tile Preview") =>
            _stretchTexture = false;

        public IntPtr SelectedTextureId { get; set; }

        public float SelectedTextureWidth { get; set; }

        public float SelectedTextureHeight { get; set; }

        protected override void DrawInternal()
        {
            if (SelectedTextureId != IntPtr.Zero)
            {
                ImGui.Checkbox("Stretch", ref _stretchTexture);

                var size = _stretchTexture
                    ? new Vector2(SelectedTextureWidth * 2, SelectedTextureHeight * 2)
                    : new Vector2(SelectedTextureWidth, SelectedTextureHeight);

                ImGui.Image(SelectedTextureId, size);
            }
        }
    }
}
