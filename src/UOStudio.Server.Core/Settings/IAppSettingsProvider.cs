namespace UOStudio.Server.Core.Settings
{
    public interface IAppSettingsProvider
    {
        AppSettings AppSettings { get; }

        void Save();

        void Load();
    }
}
