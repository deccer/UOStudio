namespace UOStudio.Core
{
    public interface IPasswordHasher
    {
        string Hash(string password, int iterations = 10000);
    }
}