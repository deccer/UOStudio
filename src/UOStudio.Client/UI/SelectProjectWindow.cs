using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Serilog;
using UOStudio.Client.Services;
using UOStudio.Common.Contracts;
using Vector4 = System.Numerics.Vector4;

namespace UOStudio.Client.UI
{
    [UsedImplicitly]
    public sealed class SelectProjectWindow : Window
    {
        private readonly INetworkClient _networkClient;
        private int _selectedProjectIndex;
        private string[] _projectNames;

        private string _errorMessage;
        private ProjectDetailDto _projectDetail;

        private int _selectedProjectIndexOld;

        public SelectProjectWindow(
            ILogger logger,
            IProjectService projectService,
            IWindowProvider windowProvider,
            INetworkClient networkClient)
            : base(windowProvider, ResGeneral.Window_Caption_SelectProject)
        {
            _networkClient = networkClient;
            _networkClient.GetProjectDetailsFailed += NetworkClientOnGetProjectDetailsFailed;
            _networkClient.GetProjectDetailsSucceeded += NetworkClientOnGetProjectDetailsSucceeded;
            _selectedProjectIndexOld = -1;
        }

        private void NetworkClientOnGetProjectDetailsSucceeded(ProjectDetailDto projectDetailDto)
        {
            _projectDetail = projectDetailDto;
        }

        private void NetworkClientOnGetProjectDetailsFailed(string errorMessage)
        {
            _errorMessage = errorMessage;
        }

        protected override void InternalDraw()
        {
            if (ImGui.ListBox("Projects", ref _selectedProjectIndex, _projectNames, _projectNames.Length))
            {
                if (_selectedProjectIndex != _selectedProjectIndexOld)
                {
                    _networkClient.GetProjectDetailsByProjectName(_projectNames[_selectedProjectIndex]);
                    _selectedProjectIndexOld = _selectedProjectIndex;
                }
            }

            if (!string.IsNullOrEmpty(_errorMessage))
            {
                ImGui.TextColored(new Vector4(1.0f, 0.0f, 0.0f, 1.0f), _errorMessage);
            }

            if (_projectDetail != null)
            {
                ImGui.TextUnformatted(_projectDetail.Name);
                ImGui.TextUnformatted(_projectDetail.Description);
                ImGui.TextUnformatted(_projectDetail.Template);
                ImGui.TextUnformatted(_projectDetail.ClientVersion);
                ImGui.TextUnformatted(_projectDetail.CreatedBy);
                ImGui.TextUnformatted(_projectDetail.CreatedAt.ToString("dd.MM.yyyy HH:mm:ss"));
            }
        }

        public void SetProjectNames(IReadOnlyList<string> projectNames)
        {
            var projectCount = projectNames.Count;
            _projectNames = new string[projectCount];
            for (var i = 0; i < projectCount; i++)
            {
                _projectNames[i] = projectNames[i];
            }
        }
    }
}
