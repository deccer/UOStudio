using System.Linq;
using ImGuiNET;
using UOStudio.Client.Core;

namespace UOStudio.Client.Engine.Windows.MapEdit
{
    public class MapConnectProfileEditorWindow : Window
    {
        private readonly ProfileService _profileService;

        private int _selectedProfileIndex;
        private Profile _selectedProfile;

        private string _profileName;
        private string _profileDescription;
        private string _profileServerName;
        private int _profileServerPort;
        private string _profileAccountName;
        private string _profileAccountPasswordHash;

        public MapConnectProfileEditorWindow(ProfileService profileService)
            : base("Edit Profiles")
        {
            _profileService = profileService;
        }

        protected override void DrawInternal()
        {
            var profiles = _profileService.GetAll();
            var profileNames = profiles.Select(p => p.Name).ToArray();

            ImGui.Columns(2);
            if (ImGui.ListBox("Profiles", ref _selectedProfileIndex, profileNames, profileNames.Length))
            {
                _selectedProfile = _selectedProfileIndex != -1 ? profiles[_selectedProfileIndex] : null;
            }

            if (ImGui.Button("Delete"))
            {
            }

            ImGui.NextColumn();

            _profileName = _selectedProfile?.Name ?? string.Empty;
            _profileDescription = _selectedProfile?.Description ?? string.Empty;
            _profileServerName = _selectedProfile?.ServerName ?? string.Empty;
            _profileServerPort = _selectedProfile?.ServerPort ?? 0;
            _profileAccountName = _selectedProfile?.AccountName ?? string.Empty;
            _profileAccountPasswordHash = _selectedProfile?.AccountPassword ?? string.Empty;

            ImGui.InputText("Name", ref _profileName, 32);
            ImGui.InputText("Description", ref _profileDescription, 256);
            ImGui.InputText("Server", ref _profileServerName, 32);
            ImGui.InputInt("Port", ref _profileServerPort);
            ImGui.InputText("Account Name", ref _profileAccountName, 128);
            ImGui.InputText("Account Password", ref _profileAccountPasswordHash, 128);

            ImGui.Separator();
            if (ImGui.Button("Add"))
            {
                var profile = new Profile
                {
                    Name = _profileName,
                    Description = _profileDescription,
                    ServerName = _profileServerName,
                    ServerPort = _profileServerPort,
                    AccountName = _profileAccountName,
                    AccountPassword = _profileAccountPasswordHash
                };
                _profileService.Add(profile);
            }
        }

        protected override ImGuiWindowFlags GetWindowFlags() => ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking;
    }
}
