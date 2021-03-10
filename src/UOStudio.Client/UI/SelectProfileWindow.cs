using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using JetBrains.Annotations;
using Serilog;
using UOStudio.Common.Contracts;

namespace UOStudio.Client.UI
{
    [UsedImplicitly]
    public sealed class SelectProfileWindow : Window
    {
        private readonly ILogger _logger;
        private readonly IWindowProvider _windowProvider;
        private readonly IProfileService _profileService;
        private readonly INetworkClient _networkClient;

        private bool _firstTimeDrawn = true;
        private string _errorMessage;
        private int _selectedProfileIndex;
        private Profile _selectedProfile;

        public SelectProfileWindow(
            ILogger logger,
            IWindowProvider windowProvider,
            IProfileService profileService,
            INetworkClient networkClient)
            : base(windowProvider, ResGeneral.Window_Caption_SelectProfile)
        {
            _logger = logger.ForContext<SelectProfileWindow>();
            _windowProvider = windowProvider;
            _profileService = profileService;
            _networkClient = networkClient;
            _networkClient.LoginSucceeded += NetworkClientOnLoginSucceeded;
            _networkClient.LoginFailed += NetworkClientOnLoginFailed;
            Show();
        }

        private void NetworkClientOnLoginFailed(string errorMessage)
        {
            _errorMessage = errorMessage;
        }

        private void NetworkClientOnLoginSucceeded(int userId, IReadOnlyCollection<ProjectDto> projects)
        {
            Hide();
            var selectProjectWindow = _windowProvider.GetWindow<SelectProjectWindow>();
            if (selectProjectWindow == null)
            {
                _logger.Error("Cant find Window {@Window}", nameof(SelectProjectWindow));
                return;
            }

            selectProjectWindow.SetProjectNames(projects.Select(p => p.Name).ToList());
            selectProjectWindow.Show();
        }

        protected override void InternalDraw()
        {
            var profileNames = _profileService.GetProfileNames();
            if (_firstTimeDrawn)
            {
                _selectedProfile = _profileService.GetProfile(profileNames[_selectedProfileIndex]);
                _firstTimeDrawn = false;
            }

            if (ImGui.Combo("##Profile", ref _selectedProfileIndex, profileNames, profileNames.Length, 5))
            {
                var selectedProfileName = profileNames[_selectedProfileIndex];
                _selectedProfile = _profileService.GetProfile(selectedProfileName);
            }

            ImGui.SameLine();
            if (ImGui.Button(ResGeneral.Button_Caption_EditProfiles))
            {
                var editProfilesWindow = _windowProvider.GetWindow<EditProfilesWindow>();
                editProfilesWindow.Show();
            }

            if (!string.IsNullOrEmpty(_errorMessage))
            {
                ImGui.TextColored(new Vector4(1, 0, 0, 1), _errorMessage);
            }

            if (_selectedProfile != null)
            {
                if (ImGui.Button(ResGeneral.Button_Caption_SelectProfile))
                {
                    _networkClient.Connect(_selectedProfile);
                }

                ImGui.SameLine();
            }

            if (ImGui.Button(ResGeneral.Button_Caption_Close))
            {
            }
        }
    }
}
