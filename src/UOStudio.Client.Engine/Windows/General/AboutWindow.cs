using ImGuiNET;

namespace UOStudio.Client.Engine.Windows.General
{
    public sealed class AboutWindow : Window
    {
        private readonly string _version;

        public AboutWindow(IFileVersionProvider fileVersionProvider)
            : base("About")
        {
            _version = fileVersionProvider.GetVersion();
        }

        protected override void DrawInternal()
        {
            ImGui.TextUnformatted($"UO Studio {_version} | Copyright 2020 deccer");
            ImGui.Separator();
            ImGui.TextUnformatted($"Edit Maps");
            ImGui.TextUnformatted($"Edit Gumps");
        }

        protected override ImGuiWindowFlags SetWindowFlags() =>
            ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoResize;
    }
}
