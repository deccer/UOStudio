using JetBrains.Annotations;

namespace UOStudio.Client.UI
{
    [UsedImplicitly]
    public sealed class CreateProjectWindow : Window
    {
        public CreateProjectWindow(IWindowProvider windowProvider)
            : base(windowProvider, "Create Project")
        {
        }

        protected override void InternalDraw()
        {
        }
    }
}
