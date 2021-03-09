namespace UOStudio.Common.Core
{
    public interface IPasswordVerifier
    {
        bool Verify(string password, string hashedPassword);
    }
}
