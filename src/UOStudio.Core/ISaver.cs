namespace UOStudio.Core
{
    public interface ISaver
    {
        void Save<T>(string fileName, T configuration) where T : class;
    }
}
