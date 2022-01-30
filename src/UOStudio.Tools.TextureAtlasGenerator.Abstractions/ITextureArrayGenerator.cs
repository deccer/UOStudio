using System;
using System.Collections.Generic;
using System.Drawing;

namespace UOStudio.Tools.TextureAtlasGenerator.Abstractions
{
    public interface ITextureArrayGenerator : IDisposable
    {
        byte[] GenerateTextureArray(IEnumerable<Bitmap> atlasPages);
    }
}
