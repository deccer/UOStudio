using System;
using System.Linq;
using ImGuiNET;
using UOStudio.Client.Core;
using Num = System.Numerics;

namespace UOStudio.Client.Engine.Windows.MapEdit
{
    public sealed class MapConnectToServerWindow : Window
    {
        private readonly IProfileService _profileService;

        public event EventHandler<ConnectEventArgs> ConnectClicked;

        public event EventHandler DisconnectClicked;

        public event Action EditProfilesClicked;

        private string _serverName;
        private string _serverPort;
        private string _userName;
        private string _password;
        private int _selectedProfileIndex;
        private Profile _selectedProfile;

        public MapConnectToServerWindow(IProfileService profileService)
            : base("Login")
        {
            Show();
            _profileService = profileService;
            _serverName = string.Empty;
            _serverPort = string.Empty;
            _userName = string.Empty;
            _password = string.Empty;
        }

        public Profile SelectedProfile
        {
            get => _selectedProfile;
            set => _selectedProfile = value;
        }

        public string ServerName
        {
            get => _serverName;
            set => _serverName = value;
        }

        public int ServerPort
        {
            get => int.TryParse(_serverPort, out var port) ? port : 0;
            set => _serverPort = value.ToString();
        }

        public string UserName
        {
            get => _userName;
            set => _userName = value;
        }

        public string Password
        {
            get => _password;
            set => _password = value;
        }

        protected override ImGuiWindowFlags GetWindowFlags() => ImGuiWindowFlags.NoDocking;

        protected override void DrawInternal()
        {
            var halfBackBufferSize = ImGui.GetWindowViewport().Size / 2.0f;
            var halfWindowSize = ImGui.GetWindowSize() / 2.0f;
            ImGui.SetWindowPos(new Num.Vector2(halfBackBufferSize.X - halfWindowSize.X, halfBackBufferSize.Y - halfWindowSize.Y));

            var profileNames = _profileService.GetProfileNames();
            if (ImGui.Combo("Profiles", ref _selectedProfileIndex, profileNames, Math.Min(5, profileNames.Length)))
            {
                var getProfileResult = _profileService.GetProfile(profileNames[_selectedProfileIndex]);
                if (getProfileResult.IsSuccess)
                {
                    _selectedProfile = getProfileResult.Value;
                    _serverName = _selectedProfile?.HostName ?? string.Empty;
                    _serverPort = _selectedProfile?.HostPort.ToString() ?? string.Empty;
                    _userName = _selectedProfile?.UserName ?? string.Empty;
                    _password = _selectedProfile?.UserPassword ?? string.Empty;
                }
            }

            if (ImGui.Button("Edit Profiles"))
            {
                var editProfilesClicked = EditProfilesClicked;
                editProfilesClicked?.Invoke();
            }

            if (ImGui.InputText("Server", ref _serverName, 64))
            {
                ServerName = _serverName;
            }

            if (ImGui.InputText("Port", ref _serverPort, 5))
            {
                ServerPort = int.TryParse(_serverPort, out var port) ? port : 0;
            }

            if (ImGui.InputText("User Name", ref _userName, 64))
            {
                UserName = _userName;
            }

            if (ImGui.InputText("Password", ref _password, 64, ImGuiInputTextFlags.Password))
            {
                Password = _password;
            }

            if (ImGui.Button("Connect"))
            {
                var serverPortAsInt = int.TryParse(_serverPort, out var port) ? port : 0;

                var connectClicked = ConnectClicked;
                connectClicked?.Invoke(this, new ConnectEventArgs(_serverName, serverPortAsInt, _userName, _password));
            }

            ImGui.SameLine();
            if (ImGui.Button("Disconnect"))
            {
                var disconnectClicked = DisconnectClicked;
                disconnectClicked?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
