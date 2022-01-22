using System.Collections.Generic;
using UOStudio.TextureAtlasGenerator.Contracts;

namespace UOStudio.TextureAtlasGenerator.Abstractions
{
    public interface IAssetExtractor
    {
        IReadOnlyCollection<TextureAsset> ExtractAssets();
    }
}
