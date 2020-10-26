using System.Numerics;
using ImGuiNET;

namespace UOStudio.Client.Engine.Windows.MapEdit
{
    public class MapToolbarWindow : Window
    {
        private readonly ToolDescription[] _toolDescriptions;

        public MapToolbarWindow(params ToolDescription[] toolDescriptions)
            : base("Controls")
        {
            _toolDescriptions = toolDescriptions;
            Show();
        }

        protected override void DrawInternal()
        {
            var currentToolGroup = ToolGroup.Selection;
            foreach (var toolDescription in _toolDescriptions)
            {
                if (currentToolGroup != toolDescription.Group)
                {
                    ImGui.Separator();
                }
                currentToolGroup = toolDescription.Group;

                ImGui.ImageButton(toolDescription.TextureHandle, new Vector2(32, 32));
                ImGui.SameLine();
            }
        }
    }
}
