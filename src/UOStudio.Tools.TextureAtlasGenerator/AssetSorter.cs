using System.Collections.Generic;
using System.Linq;
using UOStudio.Tools.TextureAtlasGenerator.Abstractions;
using UOStudio.Tools.TextureAtlasGenerator.Contracts;

namespace UOStudio.Tools.TextureAtlasGenerator
{
    internal sealed class AssetSorter : IAssetSorter
    {
        public IReadOnlyCollection<TextureAsset> SortAssets(
            IReadOnlyCollection<TextureAsset> textureAssets)
        {
            return textureAssets
                .Where(textureAsset => textureAsset.Bitmap != null)
                .OrderByDescending(textureAsset => textureAsset.Bitmap.Height)
                .ThenByDescending(textureAsset => textureAsset.Bitmap.Width * textureAsset.Bitmap.Height)
                .ToArray();
        }
    }
}
