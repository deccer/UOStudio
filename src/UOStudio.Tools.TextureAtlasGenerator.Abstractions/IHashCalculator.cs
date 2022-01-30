using System;

namespace UOStudio.Tools.TextureAtlasGenerator.Abstractions
{
    public interface IHashCalculator : IDisposable
    {
        string CalculateHash(byte[] bytes);
    }
}
