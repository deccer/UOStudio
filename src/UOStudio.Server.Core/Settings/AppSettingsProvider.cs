using UOStudio.Core;

namespace UOStudio.Server.Core.Settings
{
    public sealed class AppSettingsProvider : IAppSettingsProvider
    {
        private readonly ILoader _loader;
        private readonly ISaver _saver;

        private const string SettingsFileName = "settings.json";

        public AppSettingsProvider(
            ILoader loader,
            ISaver saver)
        {
            _loader = loader;
            _saver = saver;
            AppSettings = new AppSettings();
            Load();
        }

        public AppSettings AppSettings { get; private set; }

        public void Load()
        {
            AppSettings = _loader.Load<AppSettings>(SettingsFileName);
            if (AppSettings == null)
            {
                AppSettings = new AppSettings();
                Save();
            }
        }

        public void Save()
        {
            _saver.Save(SettingsFileName, AppSettings);
        }
    }
}
