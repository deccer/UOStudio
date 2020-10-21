using UOStudio.Core.Settings;

namespace UOStudio.Client.Core.Settings
{
    public sealed class AppSettingsProvider : IAppSettingsProvider
    {
        private readonly IConfigurationLoader _configurationLoader;
        private readonly IConfigurationSaver _configurationSaver;

        private const string SettingsFileName = "settings.json";

        public AppSettingsProvider(
            IConfigurationLoader configurationLoader,
            IConfigurationSaver configurationSaver)
        {
            _configurationLoader = configurationLoader;
            _configurationSaver = configurationSaver;
            AppSettings = new AppSettings();
            Load();
        }

        public AppSettings AppSettings { get; private set; }

        public void Load()
        {
            AppSettings = _configurationLoader.LoadConfiguration<AppSettings>(SettingsFileName);
            if (AppSettings == null)
            {
                AppSettings = new AppSettings();
                Save();
            }
        }

        public void Save()
        {
            _configurationSaver.SaveConfiguration(SettingsFileName, AppSettings);
        }
    }
}
