namespace UOStudio.Core.Settings
{
    public interface IConfigurationLoader
    {
        T LoadConfiguration<T>(string fileName) where T : class;
    }
}
