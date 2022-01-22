namespace UOStudio.TextureAtlasGenerator.Abstractions
{
    public interface IHashCalculator
    {
        string CalculateHash(byte[] bytes);
    }
}
