using System.Drawing;
using UOStudio.TextureAtlasGenerator.Abstractions;
using UOStudio.TextureAtlasGenerator.Ultima;

namespace UOStudio.TextureAtlasGenerator
{
    internal sealed class UltimaArtProvider : IUltimaArtProvider
    {
        public byte[] GetRawStatic(int artId)
            => Art.GetRawStatic(artId);

        public byte[] GetRawLand(int artId)
            => Art.GetRawLand(artId);

        public byte[] GetRawTexture(int artId)
            => Textures.TestTexture(artId)
                ? Textures.GetRawTexture(artId)
                : Textures.GetRawTexture(1);

        public Bitmap GetStatic(int artId)
            => Art.GetStatic(artId, false);

        public Bitmap GetTexture(int artId)
            => Textures.TestTexture(artId)
                ? Textures.GetTexture(artId)
                : Textures.GetTexture(1);

        public Bitmap GetLand(int artId)
            => Art.GetLand(artId);

        public void InitializeFiles(string ultimaOnlinePath)
        {
            Files.Initialize(ultimaOnlinePath);
        }
    }
}
