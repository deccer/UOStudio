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
                Salt = nonce,
                Password = passwordBytes,
                Type = Argon2Type.DataIndependentAddressing,
                Version = Argon2Version.Nineteen
            };
            using var argon = new Argon2(argonConfig);
            return argon.Hash().Buffer.SequenceEqual(hashedPassword);
        }
    }
}
