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
            var currentToolGroup = ToolGroup.Control;
            foreach (var toolDescription in _toolDescriptions)
            {
                if (currentToolGroup != toolDescription.Group)
                {
                    ImGui.Dummy(new Vector2(4, toolDescription.Size));
                    ImGui.SameLine();
                }
                currentToolGroup = toolDescription.Group;

                ImGui.ImageButton(toolDescription.TextureHandle, new Vector2(toolDescription.Size, toolDescription.Size));
                ImGui.SameLine();
            }
        }
    }
}
