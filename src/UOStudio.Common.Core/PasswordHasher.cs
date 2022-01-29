using System.Security.Cryptography;
using System.Text;
using Isopoh.Cryptography.Argon2;

namespace UOStudio.Common.Core
{
    internal sealed class PasswordHasher : IPasswordHasher
    {
        public (byte[] HashedPassword, byte[] Salt) Hash(string password)
        {
            var salt = new byte[PasswordConstants.SaltSize];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var argonConfig = new Argon2Config
            {
                Type = Argon2Type.DataIndependentAddressing,
                TimeCost = 10,
                MemoryCost = 65_536,
                Lanes = 4,
                Threads = 1,
                Version = Argon2Version.Nineteen,
                Password = passwordBytes,
                Salt = salt,
                HashLength = PasswordConstants.HashSize
            };
            using var argon = new Argon2(argonConfig);

            return (argon.Hash().Buffer, salt);
        }
    }
}
