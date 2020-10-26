using ImGuiNET;
using Microsoft.Xna.Framework.Content;
using UOStudio.Client.Engine.UI;

namespace UOStudio.Client.Engine.Windows
{
    public abstract class Window
    {
        protected Window(string caption)
        {
            Caption = caption;
        }

        public string Caption { get; set; }

        public bool IsVisible { get; private set; }

        public void Draw()
        {
            if (IsVisible && ImGui.Begin(Caption, SetWindowFlags()))
            {
                DrawInternal();
                DrawEnd();
            }
        }

        protected virtual void DrawEnd() => ImGui.End();

        protected virtual ImGuiWindowFlags SetWindowFlags() => ImGuiWindowFlags.None;

        protected abstract void DrawInternal();

        protected virtual void LoadContentInternal(ContentManager contentManager, ImGuiRenderer imGuiRenderer)
        {
        }

        public void LoadContent(ContentManager contentManager, ImGuiRenderer imGuiRenderer)
        {
            LoadContentInternal(contentManager, imGuiRenderer);
        }

        public virtual void UnloadContent()
        {
        }

        public void Hide()
        {
            IsVisible = false;
        }

        public void Show()
        {
            IsVisible = true;
        }

        public void ToggleVisibility()
        {
            if (IsVisible)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }
}
