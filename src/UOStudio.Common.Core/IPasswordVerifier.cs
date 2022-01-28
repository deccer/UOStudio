namespace UOStudio.Common.Core
{
    public interface IPasswordVerifier
    {
        bool Verify(string password, byte[] nonce, byte[] hashedPassword);
    }
}
