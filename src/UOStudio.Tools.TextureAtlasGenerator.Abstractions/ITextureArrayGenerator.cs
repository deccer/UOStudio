using System.Collections.Generic;
using System.Drawing;

namespace UOStudio.Tools.TextureAtlasGenerator.Abstractions
{
    public interface ITextureArrayGenerator
    {
        byte[] GenerateTextureArray(IEnumerable<Bitmap> atlasPages);
    }
}
