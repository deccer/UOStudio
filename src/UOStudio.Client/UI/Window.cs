using ImGuiNET;

namespace UOStudio.Client.UI
{
    public abstract class Window
    {
        protected Window(
            IWindowProvider windowProvider,
            string caption)
        {
            Caption = caption;
            WindowProvider = windowProvider;
        }

        public string Caption { get; set; }

        public int Id { get; }

        public bool IsVisible { get; private set; }

        protected IWindowProvider WindowProvider { get; }

        protected virtual ImGuiWindowFlags WindowSettings => ImGuiWindowFlags.None;

        public void Draw()
        {
            if (IsVisible && ImGui.Begin(Caption, WindowSettings))
            {
                InternalDraw();
                ImGui.End();
            }
        }

        public void Hide()
            => IsVisible = false;

        public void Show()
            => IsVisible = true;

        protected abstract void InternalDraw();
    }
}
