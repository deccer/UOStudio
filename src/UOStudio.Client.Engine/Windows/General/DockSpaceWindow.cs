using ImGuiNET;
using Num = System.Numerics;

namespace UOStudio.Client.Engine.Windows.General
{
    public sealed class DockSpaceWindow : Window
    {
        private bool _hasPadding = false;
        private ImGuiDockNodeFlags _dockspaceFlags = ImGuiDockNodeFlags.None;

        public DockSpaceWindow(string caption)
            : base(caption)
        {
            Show();
        }

        protected override ImGuiWindowFlags SetWindowFlags()
        {
            var windowFlags = ImGuiWindowFlags.NoDocking;
            var viewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(viewport.GetWorkPos());
            ImGui.SetNextWindowSize(viewport.GetWorkSize());
            ImGui.SetNextWindowViewport(viewport.ID);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            windowFlags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse |
                           ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove |
                           ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;

            if ((_dockspaceFlags & ImGuiDockNodeFlags.PassthruCentralNode) == ImGuiDockNodeFlags.PassthruCentralNode)
            {
                windowFlags |= ImGuiWindowFlags.NoBackground;
            }

            if (!_hasPadding)
            {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Num.Vector2(0.0f, 0.0f));
            }

            return windowFlags;
        }

        protected override void DrawInternal()
        {
            if (!_hasPadding)
            {
                ImGui.PopStyleVar();
            }

            ImGui.PopStyleVar(2);

            var io = ImGui.GetIO();
            if ((io.ConfigFlags & ImGuiConfigFlags.DockingEnable) == ImGuiConfigFlags.DockingEnable)
            {
                var dockspaceId = ImGui.GetID(Caption);
                ImGui.DockSpace(dockspaceId, new Num.Vector2(-1f, -1f), _dockspaceFlags);
            }
        }

        protected override void DrawEnd()
        {
        }
    }
}
