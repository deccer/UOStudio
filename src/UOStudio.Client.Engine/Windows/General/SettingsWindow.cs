using System.Globalization;
using System.Threading;
using ImGuiNET;
using UOStudio.Client.Core.Settings;
using UOStudio.Client.Engine.Resources;

namespace UOStudio.Client.Engine.Windows.General
{
    public class SettingsWindow : Window
    {
        private readonly IAppSettingsProvider _appSettingsProvider;

        public SettingsWindow(IAppSettingsProvider appSettingsProvider)
            : base(ResGeneral.WindowCaptionSettings)
        {
            _appSettingsProvider = appSettingsProvider;
        }

        protected override void DrawInternal()
        {
            if (ImGui.Button("en"))
            {
                ResGeneral.Culture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = ResGeneral.Culture = Thread.CurrentThread.CurrentCulture;
            }

            if (ImGui.Button("de"))
            {
                ResGeneral.Culture = new CultureInfo("de-DE");
                Thread.CurrentThread.CurrentUICulture = ResGeneral.Culture = Thread.CurrentThread.CurrentCulture;
            }
            ImGui.End();
        }
    }
}
