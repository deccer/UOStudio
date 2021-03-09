using System;
using System.Security.Cryptography;

namespace UOStudio.Common.Core
{
    public class PasswordVerifier : IPasswordVerifier
    {
        public bool Verify(string password, string hashedPassword)
        {
            if (!IsHashSupported(hashedPassword))
            {
                throw new NotSupportedException("The hash type is not supported");
            }

            var splitHashString = hashedPassword.Replace("$HP$V1$", "").Split('$');
            var iterations = int.Parse(splitHashString[0]);
            var base64Hash = splitHashString[1];

            var hashBytes = Convert.FromBase64String(base64Hash);

            var salt = new byte[PasswordConstants.SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, PasswordConstants.SaltSize);

            using var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, iterations);
            var hash = rfc2898DeriveBytes.GetBytes(PasswordConstants.HashSize);

            for (var i = 0; i < PasswordConstants.HashSize; i++)
            {
                if (hashBytes[i + PasswordConstants.SaltSize] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsHashSupported(string hashString) => hashString.Contains("$HP$V1$");
    }
}
