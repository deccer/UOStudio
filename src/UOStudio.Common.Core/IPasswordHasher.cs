namespace UOStudio.Common.Core
{
    public interface IPasswordHasher
    {
        string Hash(string password, int iterations = 10000);
    }
}
