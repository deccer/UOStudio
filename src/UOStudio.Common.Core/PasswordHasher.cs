using System;
using System.Security.Cryptography;

namespace UOStudio.Common.Core
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password, int iterations = 10000)
        {
            var salt = new byte[PasswordConstants.SaltSize];
            using var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(salt);

            using var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, iterations);
            var hash = rfc2898DeriveBytes.GetBytes(PasswordConstants.HashSize);

            var hashBytes = new byte[PasswordConstants.SaltSize + PasswordConstants.HashSize];
            Array.Copy(salt, 0, hashBytes, 0, PasswordConstants.SaltSize);
            Array.Copy(hash, 0, hashBytes, PasswordConstants.SaltSize, PasswordConstants.HashSize);

            var base64Hash = Convert.ToBase64String(hashBytes);

            return $"$HP$V1${iterations}${base64Hash}";
        }
    }
}
