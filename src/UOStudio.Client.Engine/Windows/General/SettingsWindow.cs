using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using ImGuiNET;
using UOStudio.Client.Core.Settings;
using UOStudio.Client.Resources;

namespace UOStudio.Client.Engine.Windows.General
{
    public class SettingsWindow : Window
    {
        private readonly IAppSettingsProvider _appSettingsProvider;

        private readonly IDictionary<string, CultureInfo> _cultures;

        public SettingsWindow(IAppSettingsProvider appSettingsProvider)
            : base(ResGeneral.WindowCaptionSettings)
        {
            _cultures = new Dictionary<string, CultureInfo>
            {
                { Constants.Language.English, new CultureInfo("en-US") },
                { Constants.Language.German, new CultureInfo("de-DE") }
            };
            _appSettingsProvider = appSettingsProvider;
        }

        protected override void DrawInternal()
        {
            if (ImGui.BeginTabBar("Settings"))
            {
                if (ImGui.BeginTabItem("General"))
                {
                    var uoPath = _appSettingsProvider.AppSettings.General.UltimaOnlineBasePath;
                    ImGui.TextUnformatted("Path to UO");
                    ImGui.SameLine();
                    if (ImGui.InputText("##pathbox", ref uoPath, 240) && Directory.Exists(uoPath))
                    {
                        _appSettingsProvider.AppSettings.General.UltimaOnlineBasePath = uoPath;
                    }

                    ImGui.Separator();

                    if (ImGui.Button("English"))
                    {
                        SetCulture(Constants.Language.English);
                    }

                    if (ImGui.Button("German"))
                    {
                        SetCulture(Constants.Language.German);
                    }

                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }

            ImGui.End();
        }

        private void SetCulture(string language)
        {
            ResGeneral.Culture = _cultures.TryGetValue(language, out var culture)
                ? culture
                : CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = ResGeneral.Culture;
        }
    }
}
