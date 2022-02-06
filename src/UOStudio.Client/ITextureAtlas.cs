using System;
using UOStudio.Client.Engine.Graphics;
using UOStudio.Tools.TextureAtlasGenerator.Contracts;

namespace UOStudio.Client
{
    public interface ITextureAtlas
        : IDisposable
    {
        int Depth { get; }

        ITextureArray AtlasTexture { get; }

        ITextureView[] AtlasTextureViews { get; }

        int LandTileCount { get; }

        int LandTextureCount { get; }

        int ItemTileCount { get; }

        LandTile GetLandTile(int landId);

        LandTile GetLandTextureTile(int landId);

        ItemTile GetItemTile(int staticId);

        bool Load();
    }
}
