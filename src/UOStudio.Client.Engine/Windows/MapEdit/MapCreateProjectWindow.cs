using System;
using ImGuiNET;

namespace UOStudio.Client.Engine.Windows.MapEdit
{
    public class MapCreateProjectWindow : Window
    {
        private string _projectName;
        private string _projectDescription;
        private string _projectClientVersion;

        public MapCreateProjectWindow()
            : base("Create Project")
        {
            _projectName = string.Empty;
            _projectDescription = string.Empty;
            _projectClientVersion = string.Empty;
        }

        public event Action CreateProjectClicked;

        public string Name
        {
            get => _projectName;
            set => _projectName = value;
        }

        public string Description
        {
            get => _projectDescription;
            set => _projectDescription = value;
        }

        public string ClientVersion
        {
            get => _projectClientVersion;
            set => _projectClientVersion = value;
        }

        protected override void DrawInternal()
        {
            ImGui.SetNextItemWidth(80.0f);
            ImGui.TextUnformatted("Name");
            ImGui.SameLine(128);
            ImGui.SetNextItemWidth(256);
            ImGui.InputText("##name", ref _projectName, 64);

            ImGui.SetNextItemWidth(80.0f);
            ImGui.TextUnformatted("Description");
            ImGui.SameLine(128);
            ImGui.SetNextItemWidth(256);
            ImGui.InputText("##description", ref _projectDescription, 64);

            ImGui.SetNextItemWidth(80.0f);
            ImGui.TextUnformatted("ClientVersion");
            ImGui.SameLine(128);
            ImGui.SetNextItemWidth(256);
            ImGui.InputText("##clientVersion", ref _projectClientVersion, 64);

            ImGui.Separator();
            if (ImGui.Button("Create"))
            {
                var createProjectClicked = CreateProjectClicked;
                createProjectClicked?.Invoke();
            }

            ImGui.SameLine();
            if (ImGui.Button("Cancel"))
            {
                Hide();
            }
        }

        protected override ImGuiWindowFlags GetWindowFlags() => base.GetWindowFlags() | ImGuiWindowFlags.NoDocking;
    }
}
