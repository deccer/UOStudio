using ImGuiNET;
using JetBrains.Annotations;
using Serilog;

namespace UOStudio.Client.UI
{
    [UsedImplicitly]
    public sealed class EditProfilesWindow : Window
    {
        private readonly ILogger _logger;
        private readonly IProfileService _profileService;

        private int _selectedProfileIndex;
        private Profile _selectedProfile;

        private string _profileName;
        private string _profileServerName;
        private int _profileServerPort;
        private string _profileUserName;
        private string _profilePassword;

        public EditProfilesWindow(
            ILogger logger,
            IWindowProvider windowProvider,
            IProfileService profileService)
            : base(windowProvider, ResGeneral.Window_Caption_EditProfiles)
        {
            _logger = logger.ForContext<EditProfilesWindow>();
            _profileService = profileService;
        }

        protected override void InternalDraw()
        {
            var profileNames = _profileService.GetProfileNames();
            if (ImGui.ListBox(ResGeneral.Label_Caption_Profiles, ref _selectedProfileIndex, profileNames, profileNames.Length))
            {
                _selectedProfile = _profileService.GetProfile(profileNames[_selectedProfileIndex]);

                _profileName = _selectedProfile.Name;
                _profileServerName = _selectedProfile.ServerName;
                _profileServerPort = _selectedProfile.ServerPort;
                _profileUserName = _selectedProfile.UserName;
                _profilePassword = _selectedProfile.Password;
            }

            if (_selectedProfile != null)
            {
                ImGui.InputText(ResGeneral.Label_Caption_Name, ref _profileName, 128);
                ImGui.InputText(ResGeneral.Label_Caption_Host, ref _profileServerName, 128);
                ImGui.InputInt(ResGeneral.Label_Caption_Port, ref _profileServerPort, 1, 100);
                ImGui.InputText(ResGeneral.Label_Caption_UserName, ref _profileUserName, 128);
                ImGui.InputText(ResGeneral.Label_Caption_Password, ref _profilePassword, 256);
            }

            if (ImGui.Button(ResGeneral.Button_Caption_DeleteProfile))
            {
                _profileService.DeleteProfile(_selectedProfile);
            }

            ImGui.SameLine();
            if (ImGui.Button(ResGeneral.Button_Caption_AddProfile))
            {
                _profileService.CreateProfile(
                    _profileName,
                    _profileServerName,
                    _profileServerPort,
                    _profileUserName,
                    _profilePassword);
            }

            ImGui.SameLine();
            if (ImGui.Button(ResGeneral.Button_Caption_Close))
            {
                Hide();
            }
        }
    }
}
