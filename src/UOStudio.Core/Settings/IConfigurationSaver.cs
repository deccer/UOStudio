namespace UOStudio.Core.Settings
{
    public interface IConfigurationSaver
    {
        void SaveConfiguration<T>(string fileName, T configuration) where T : class;
    }
}
