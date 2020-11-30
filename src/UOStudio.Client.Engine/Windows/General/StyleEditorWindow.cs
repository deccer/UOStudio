using ImGuiNET;

namespace UOStudio.Client.Engine.Windows.General
{
    public sealed class StyleEditorWindow : Window
    {
        public StyleEditorWindow()
            : base("Edit Style")
        {
        }

        protected override void DrawInternal()
        {
            var currentStyle = ImGui.GetStyle();
            ImGui.ShowStyleEditor(currentStyle);
            ImGui.End();
        }
    }
}
