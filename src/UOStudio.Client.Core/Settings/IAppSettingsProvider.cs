namespace UOStudio.Client.Core.Settings
{
    public interface IAppSettingsProvider
    {
        AppSettings AppSettings { get; }

        void Save();

        void Load();
    }
}
