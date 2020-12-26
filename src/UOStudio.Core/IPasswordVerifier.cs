namespace UOStudio.Core
{
    public interface IPasswordVerifier
    {
        bool Verify(string password, string hashedPassword);
    }
}