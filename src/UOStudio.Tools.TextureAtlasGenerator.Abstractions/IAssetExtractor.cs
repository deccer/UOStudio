using System.Collections.Generic;
using UOStudio.Tools.TextureAtlasGenerator.Contracts;

namespace UOStudio.Tools.TextureAtlasGenerator.Abstractions
{
    public interface IAssetExtractor
    {
        IReadOnlyCollection<TextureAsset> ExtractAssets();
    }
}
