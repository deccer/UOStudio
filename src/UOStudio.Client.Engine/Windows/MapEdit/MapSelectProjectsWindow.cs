using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using UOStudio.Client.Core;

namespace UOStudio.Client.Engine.Windows.MapEdit
{
    public sealed class MapSelectProjectsWindow : Window
    {
        private readonly IList<Project> _projects;
        private readonly IList<string> _projectNames;
        private int _selectedIndex;

        public MapSelectProjectsWindow()
            : base("Select Project")
        {
            _projects = new List<Project>(16);
            _projectNames = new List<string>(16);
            _selectedIndex = -1;
        }

        public Project SelectedProject { get; set; }

        public event Action<Project> SelectProjectClicked;

        public event Action<Project> DeleteProjectClicked;

        public event Action CreateProjectClicked;

        public event Action RefreshProjectListClicked;

        protected override void DrawInternal()
        {
            if (ImGui.Button("Refresh"))
            {
                var refreshProjectListClicked = RefreshProjectListClicked;
                refreshProjectListClicked?.Invoke();
            }

            ImGui.Separator();

            ImGui.TextUnformatted("Projects");
            ImGui.SetNextWindowSize(new Vector2(-1, 200));
            ImGui.ListBox("", ref _selectedIndex, _projectNames.ToArray(), _projectNames.Count);
            if (_selectedIndex > -1 && _selectedIndex < _projects.Count)
            {
                SelectedProject = _projects[_selectedIndex];
            }

            ImGui.Separator();

            if (ImGui.Button("Select"))
            {
                var selectProjectClicked = SelectProjectClicked;
                selectProjectClicked?.Invoke(SelectedProject);

                Hide();
            }

            ImGui.SameLine();
            if (ImGui.Button("Delete"))
            {
                var deleteProjectClicked = DeleteProjectClicked;
                deleteProjectClicked?.Invoke(SelectedProject);
            }

            ImGui.SameLine();
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

        public void SetProjects(IEnumerable<Project> projects)
        {
            if (projects == null)
            {
                return;
            }

            _projects.Clear();
            _projectNames.Clear();
            foreach (var project in projects)
            {
                _projects.Add(project);
                _projectNames.Add(project.Name);
            }
        }
    }
}
