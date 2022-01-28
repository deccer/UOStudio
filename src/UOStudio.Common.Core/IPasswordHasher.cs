namespace UOStudio.Common.Core
{
    public interface IPasswordHasher
    {
        (byte[] HashedPassword, byte[] Salt) Hash(string password);
    }
}
