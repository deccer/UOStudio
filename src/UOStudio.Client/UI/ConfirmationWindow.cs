using ImGuiNET;

namespace UOStudio.Client.UI
{
    public sealed class ConfirmationWindow : Window
    {
        private string _dialog;
        private DialogButtons _dialogButtons;

        public ConfirmationWindow(IWindowProvider windowProvider)
            : base(windowProvider, "Confirmation")
        {
        }

        public DialogButtons DialogButtons
        {
            get => _dialogButtons;
            set => _dialogButtons = value;
        }

        public string Dialog
        {
            get => _dialog;
            set => _dialog = value ?? string.Empty;
        }

        public DialogResult DialogResult { get; private set; }

        protected override void InternalDraw()
        {
            ImGui.TextUnformatted(_dialog);

            if ((_dialogButtons & DialogButtons.Yes) == DialogButtons.Yes && ImGui.Button("Yes"))
            {
                DialogResult = DialogResult.Yes;
                Hide();
            }

            ImGui.SameLine();
            if ((_dialogButtons & DialogButtons.No) == DialogButtons.No && ImGui.Button("No"))
            {
                DialogResult = DialogResult.No;
                Hide();
            }

            ImGui.SameLine();
            if ((_dialogButtons & DialogButtons.OK) == DialogButtons.OK && ImGui.Button("OK"))
            {
                DialogResult = DialogResult.OK;
                Hide();
            }

            ImGui.SameLine();
            if ((_dialogButtons & DialogButtons.Cancel) == DialogButtons.Cancel && ImGui.Button("Cancel"))
            {
                DialogResult = DialogResult.Cancel;
                Hide();
            }
        }
    }
}
