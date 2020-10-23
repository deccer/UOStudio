namespace UOStudio.Core
{
    public interface ILoader
    {
        T Load<T>(string fileName) where T : class;
    }
}
