using System;
using System.Linq;
using ImGuiNET;
using UOStudio.Client.Core;

namespace UOStudio.Client.Engine.Windows.MapEdit
{
    public class MapViewProfileWindow : Window
    {
        private readonly ProfileService _profileService;

        private int _selectedProfileIndex;
        private Profile _selectedProfile;

        public MapViewProfileWindow(ProfileService profileService)
            : base("Edit Profiles") =>
            _profileService = profileService;

        public event Action AddProfileClicked;

        public event Action<Profile> DeleteProfileClicked;

        public event Action<Profile> UpdateProfileClicked;

        protected override void DrawInternal()
        {
            var profiles = _profileService.GetAll();
            var profileNames = profiles.Select(p => p.Name).ToArray();

            ImGui.BeginGroup();
            {
                ImGui.TextUnformatted("Profiles");
                ImGui.SetNextItemWidth(-1);
                if (ImGui.ListBox(string.Empty, ref _selectedProfileIndex, profileNames, profileNames.Length))
                {
                    _selectedProfile = _selectedProfileIndex != -1 ? profiles[_selectedProfileIndex] : null;
                }

                var profileName = _selectedProfile?.Name ?? string.Empty;
                var profileDescription = _selectedProfile?.Description ?? string.Empty;
                var profileServerName = _selectedProfile?.ServerName ?? string.Empty;
                var profileServerPort = _selectedProfile?.ServerPort ?? 0;
                var profileAccountName = _selectedProfile?.AccountName ?? string.Empty;
                var profileAccountPasswordHash = _selectedProfile?.AccountPassword ?? string.Empty;

                ImGui.TextUnformatted("Name");
                ImGui.SameLine(96);
                ImGui.TextUnformatted(profileName);

                ImGui.TextUnformatted("Description");
                ImGui.SameLine(96);
                ImGui.TextUnformatted(profileDescription);

                ImGui.Separator();
                ImGui.TextUnformatted("Server");
                ImGui.SameLine(96);
                ImGui.TextUnformatted(profileServerName);

                ImGui.TextUnformatted("Port");
                ImGui.SameLine(96);
                ImGui.TextUnformatted(profileServerPort.ToString());

                ImGui.Separator();

                ImGui.TextUnformatted("UserName");
                ImGui.SameLine(96);
                ImGui.TextUnformatted(profileAccountName);

                ImGui.TextUnformatted("Password");
                ImGui.SameLine(96);
                ImGui.TextUnformatted(profileAccountPasswordHash);

                ImGui.EndGroup();
            }

            ImGui.Separator();

            if (ImGui.Button("Add"))
            {
                var addProfileClicked = AddProfileClicked;
                addProfileClicked?.Invoke();
            }

            ImGui.SameLine();
            if (ImGui.Button("Delete"))
            {
                var deleteProfileClicked = DeleteProfileClicked;
                deleteProfileClicked?.Invoke(_selectedProfile);
            }

            ImGui.SameLine();
            if (ImGui.Button("Update"))
            {
                var updateProfileClicked = UpdateProfileClicked;
                updateProfileClicked?.Invoke(_selectedProfile);
            }

            ImGui.SameLine();
            if (ImGui.Button("Close"))
            {
                Hide();
            }
        }

        protected override ImGuiWindowFlags GetWindowFlags() => ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking;
    }
}
