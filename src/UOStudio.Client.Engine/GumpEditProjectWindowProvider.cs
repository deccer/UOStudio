using UOStudio.Client.Core.Settings;
using UOStudio.Client.Engine.Windows.General;

namespace UOStudio.Client.Engine.Windows
{
    public sealed class GumpEditProjectWindowProvider : CommonProjectWindowProvider
    {
        public GumpEditProjectWindowProvider(IAppSettingsProvider appSettingsProvider, IFileVersionProvider fileVersionProvider)
            : base(appSettingsProvider, fileVersionProvider)
        {
            DockSpaceWindow = new DockSpaceWindow("GumpEdit");
        }

        public DockSpaceWindow DockSpaceWindow { get; }

        public override void Draw()
        {
            DockSpaceWindow.Draw();
            base.Draw();
        }
    }
}
