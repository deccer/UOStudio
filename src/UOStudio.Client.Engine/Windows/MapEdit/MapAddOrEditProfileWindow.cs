using ImGuiNET;
using UOStudio.Client.Core;

namespace UOStudio.Client.Engine.Windows.MapEdit
{
    public class MapAddOrEditProfileWindow : Window
    {
        private readonly ProfileService _profileService;
        private string _profileName;
        private string _profileDescription;
        private string _profileServerName;
        private int _profileServerPort;
        private string _profileAccountName;
        private string _profileAccountPassword;

        public MapAddOrEditProfileWindow(ProfileService profileService)
            : base(string.Empty)
        {
            _profileService = profileService;
            _profileName = string.Empty;
            _profileDescription = string.Empty;
            _profileServerName = string.Empty;
            _profileServerPort = 0;
            _profileAccountName = string.Empty;
            _profileAccountPassword = string.Empty;
        }

        public ProfileAddOrEdit AddOrEdit { get; set; }

        public Profile SelectedProfile { get; set; }

        protected override void DrawInternal()
        {
            Caption = AddOrEdit == ProfileAddOrEdit.Add
                ? $"{nameof(ProfileAddOrEdit.Add)} Profile"
                : $"{nameof(ProfileAddOrEdit.Edit)} Profile";

            var profileName = AddOrEdit == ProfileAddOrEdit.Edit
                ? SelectedProfile?.Name ?? string.Empty
                : _profileName;
            var profileDescription = AddOrEdit == ProfileAddOrEdit.Edit
                ? SelectedProfile?.Description ?? string.Empty
                : _profileDescription;
            var profileServerName = AddOrEdit == ProfileAddOrEdit.Edit
                ? SelectedProfile?.ServerName ?? string.Empty
                : _profileServerName;
            var profileServerPort = AddOrEdit == ProfileAddOrEdit.Edit
                ? SelectedProfile?.ServerPort ?? 0
                : _profileServerPort;
            var profileAccountName = AddOrEdit == ProfileAddOrEdit.Edit
                ? SelectedProfile?.AccountName ?? string.Empty
                : _profileAccountName;
            var profileAccountPasswordHash = AddOrEdit == ProfileAddOrEdit.Edit
                ? SelectedProfile?.AccountPassword ?? string.Empty
                : _profileAccountPassword;

            if (ImGui.InputText("Name", ref profileName, 256))
            {
                _profileName = profileName;
            }

            if (ImGui.InputText("Description", ref profileDescription, 256))
            {
                _profileDescription = profileDescription;
            }

            ImGui.Separator();

            if (ImGui.InputText("Server", ref profileServerName, 32))
            {
                _profileServerName = profileServerName;
            }

            if (ImGui.InputInt("Port", ref profileServerPort))
            {
                _profileServerPort = profileServerPort;
            }

            ImGui.Separator();

            if (ImGui.InputText("Account Name", ref profileAccountName, 128))
            {
                _profileAccountName = profileAccountName;
            }

            if (ImGui.InputText("Account Password", ref profileAccountPasswordHash, 128))
            {
                _profileAccountPassword = profileAccountPasswordHash;
            }

            ImGui.Separator();

            if (ImGui.Button(AddOrEdit == ProfileAddOrEdit.Add ? "Add" : "Update"))
            {
                if (AddOrEdit == ProfileAddOrEdit.Add)
                {
                    var profile = new Profile
                    {
                        Name = _profileName,
                        Description = _profileDescription,
                        ServerName = _profileServerName,
                        ServerPort = _profileServerPort,
                        AccountName = _profileAccountName,
                        AccountPassword = _profileAccountPassword
                    };
                    _profileService.Add(profile);
                }
                else if (AddOrEdit == ProfileAddOrEdit.Edit)
                {
                    SelectedProfile.Name = _profileName;
                    SelectedProfile.Description = _profileDescription;
                    SelectedProfile.ServerName = _profileServerName;
                    SelectedProfile.ServerPort = _profileServerPort;
                    SelectedProfile.AccountName = _profileAccountName;
                    SelectedProfile.AccountPassword = _profileAccountPassword;

                    _profileService.Update(SelectedProfile);
                }
            }

            ImGui.SameLine();
            if (ImGui.Button("Close"))
            {
                Hide();
            }
        }
    }
}
