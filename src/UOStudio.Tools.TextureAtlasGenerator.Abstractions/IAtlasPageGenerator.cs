using System.Collections.Generic;
using System.Drawing;
using UOStudio.Tools.TextureAtlasGenerator.Contracts;

namespace UOStudio.Tools.TextureAtlasGenerator.Abstractions
{
    public interface IAtlasPageGenerator
    {
        IReadOnlyCollection<Bitmap> GeneratePages(IReadOnlyList<TextureAsset> textureAssets);
    }
}
