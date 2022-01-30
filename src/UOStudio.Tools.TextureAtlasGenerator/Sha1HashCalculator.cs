using System;
using System.Security.Cryptography;
using UOStudio.Tools.TextureAtlasGenerator.Abstractions;

namespace UOStudio.Tools.TextureAtlasGenerator
{
    internal sealed class Sha1HashCalculator : IHashCalculator
    {
        public string CalculateHash(byte[] bytes)
        {
            using var sha1 = SHA1.Create();
            return Convert.ToHexString(sha1.ComputeHash(bytes));
        }
    }
}
