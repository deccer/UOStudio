using System.Linq;
using System.Text;
using Isopoh.Cryptography.Argon2;

namespace UOStudio.Common.Core
{
    internal sealed class PasswordVerifier : IPasswordVerifier
    {
        public bool Verify(string password, byte[] nonce, byte[] hashedPassword)
        {
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
                Salt = nonce,
                HashLength = PasswordConstants.HashSize
            };

            using var argon = new Argon2(argonConfig);
            return argon.Hash().Buffer.SequenceEqual(hashedPassword);
        }
    }
}
