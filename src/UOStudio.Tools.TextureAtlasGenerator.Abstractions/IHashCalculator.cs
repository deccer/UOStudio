namespace UOStudio.Tools.TextureAtlasGenerator.Abstractions
{
    public interface IHashCalculator
    {
        string CalculateHash(byte[] bytes);
    }
}
