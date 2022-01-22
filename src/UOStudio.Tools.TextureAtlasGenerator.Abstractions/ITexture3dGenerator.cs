using System;
using System.Collections.Generic;
using System.Drawing;

namespace UOStudio.TextureAtlasGenerator.Abstractions
{
    public interface ITexture3dGenerator : IDisposable
    {
        byte[] Generate3dTexture(IEnumerable<Bitmap> atlasPages);
    }
}
