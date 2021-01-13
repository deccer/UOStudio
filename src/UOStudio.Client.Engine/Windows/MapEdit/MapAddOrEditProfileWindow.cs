using ImGuiNET;

namespace UOStudio.Client.Engine.Windows.MapEdit
{
    public class MapAddOrEditProfileWindow : Window
    {
        private readonly IProfileService _profileService;
        private string _profileName;
        private string _profileServerName;
        private int _profileServerPort;
        private string _profileAccountName;
        private string _profileAccountPassword;

        public MapAddOrEditProfileWindow(IProfileService profileService)
            : base(string.Empty)
        {
            _profileService = profileService;
            _profileName = string.Empty;
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
            var profileServerName = AddOrEdit == ProfileAddOrEdit.Edit
                ? SelectedProfile?.HostName ?? string.Empty
                : _profileServerName;
            var profileServerPort = AddOrEdit == ProfileAddOrEdit.Edit
                ? SelectedProfile?.HostPort ?? 0
                : _profileServerPort;
            var profileAccountName = AddOrEdit == ProfileAddOrEdit.Edit
                ? SelectedProfile?.UserName ?? string.Empty
                : _profileAccountName;
            var profileAccountPasswordHash = AddOrEdit == ProfileAddOrEdit.Edit
                ? SelectedProfile?.UserPassword ?? string.Empty
                : _profileAccountPassword;

            if (ImGui.InputText("Name", ref profileName, 256))
            {
                _profileName = profileName;
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
                        HostName = _profileServerName,
                        HostPort = _profileServerPort,
                        UserName = _profileAccountName,
                        UserPassword = _profileAccountPassword
                    };
                    _profileService.AddProfile(profile);
                }
                else if (AddOrEdit == ProfileAddOrEdit.Edit)
                {
                    SelectedProfile!.Name = _profileName;
                    SelectedProfile.HostName = _profileServerName;
                    SelectedProfile.HostPort = _profileServerPort;
                    SelectedProfile.UserName = _profileAccountName;
                    SelectedProfile.UserPassword = _profileAccountPassword;

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
