using System;
using ImGuiNET;

namespace UOStudio.Client.Engine.Windows
{
    public class LoginWindow : Window
    {
        public event EventHandler<ConnectEventArgs> OnConnect;

        public event EventHandler OnDisconnect;

        private string _serverName;

        public string ServerName
        {
            get => _serverName;
            set => _serverName = value;
        }

        private string _serverPort;

        public int ServerPort
        {
            get => int.TryParse(_serverPort, out var port) ? port : 0;
            set => _serverPort = value.ToString();
        }

        private string _userName;

        public string UserName
        {
            get => _userName;
            set => _userName = value;
        }

        private string _password;

        public string Password
        {
            get => _password;
            set => _password = value;
        }

        public LoginWindow()
            : base("Login")
        {
            _serverName = string.Empty;
            _serverPort = string.Empty;
            _userName = string.Empty;
            _password = string.Empty;
        }

        protected override ImGuiWindowFlags SetWindowFlags() => ImGuiWindowFlags.NoCollapse;

        protected override void DrawInternal()
        {
            ImGui.BeginGroup();

            ImGui.TextUnformatted("Server");
            ImGui.SameLine();
            ImGui.InputText("##hidelabel", ref _serverName, 64);

            ImGui.TextUnformatted("Port");
            ImGui.SameLine();
            ImGui.InputText("##hidelabel", ref _serverPort, 5);

            ImGui.TextUnformatted("Username");
            ImGui.SameLine();
            ImGui.InputText("##hidelabel", ref _userName, 5);

            ImGui.TextUnformatted("Password");
            ImGui.SameLine();
            ImGui.InputText("##hidelabel", ref _password, 5, ImGuiInputTextFlags.Password);

            ImGui.EndGroup();

            if (ImGui.Button("Connect"))
            {
                var serverPort = int.TryParse(_serverPort, out var port) ? port : 0;

                OnConnect?.Invoke(this, new ConnectEventArgs(_serverName, serverPort, _userName, _password));
            }

            if (ImGui.Button("Disconnect"))
            {
                OnDisconnect?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
