using System.Collections.Generic;
using UOStudio.TextureAtlasGenerator.Contracts;

namespace UOStudio.TextureAtlasGenerator.Abstractions
{
    public interface IAssetSorter
    {
        IReadOnlyCollection<TextureAsset> SortAssets(IReadOnlyCollection<TextureAsset> textureAssets);
    }
}
