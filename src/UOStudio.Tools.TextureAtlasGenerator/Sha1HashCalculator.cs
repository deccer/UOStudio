using System;
using System.Security.Cryptography;
using UOStudio.Tools.TextureAtlasGenerator.Abstractions;

namespace UOStudio.Tools.TextureAtlasGenerator
{
    internal sealed class Sha1HashCalculator : IHashCalculator
    {
        private readonly HashAlgorithm _hashAlgorithm;

        public Sha1HashCalculator()
        {
            _hashAlgorithm = SHA1.Create();
        }

        public void Dispose()
        {
            _hashAlgorithm.Dispose();
        }

        public string CalculateHash(byte[] bytes)
        {
            return Convert.ToHexString(_hashAlgorithm.ComputeHash(bytes));
        }
    }
}
