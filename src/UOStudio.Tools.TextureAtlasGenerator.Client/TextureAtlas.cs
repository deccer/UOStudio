using System;
using System.Collections.Generic;
using Serilog;
using UOStudio.Client.Engine.Graphics;
using UOStudio.TextureAtlasGenerator.Contracts;

namespace UOStudio.TextureAtlasGenerator.Client
{
    public class TextureAtlas : IDisposable
    {
        private readonly LandTile _invalidLandTile;
        private readonly ItemTile _invalidItemTile;

        private readonly ILogger _logger;
        private readonly IGraphicsDevice _graphicsDevice;
        private readonly string _atlasName;

        private IDictionary<int, LandTile> _landTiles;
        private IDictionary<int, LandTile> _landTextureTiles;
        private IDictionary<int, ItemTile> _itemTiles;
        private int _depth;

        public int Depth => _depth;

        public ITextureArray AtlasTexture { get; private set; }

        public TextureAtlas(ILogger logger, IGraphicsDevice graphicsDevice, string atlasName)
        {
            _logger = logger.ForContext<TextureAtlas>();
            _graphicsDevice = graphicsDevice;
            _atlasName = atlasName;
            _invalidLandTile = new LandTile(-1, default);
            _invalidItemTile = new ItemTile(default, default);

            _landTiles = new Dictionary<int, LandTile>();
            _landTextureTiles = new Dictionary<int, LandTile>();
            _itemTiles = new Dictionary<int, ItemTile>();
        }

        public void Dispose()
        {
            AtlasTexture?.Dispose();
        }

        public LandTile GetLandTile(int landId)
            => _landTiles.TryGetValue(landId, out var landTile)
                ? landTile
                : _invalidLandTile;

        public LandTile GetLandTextureTile(int landId)
            => _landTextureTiles.TryGetValue(landId, out var landTile)
                ? landTile
                : _invalidLandTile;

        public ItemTile GetItemTile(int staticId)
            => _itemTiles.TryGetValue(staticId, out var staticTile)
                ? staticTile
                : _invalidItemTile;

        public void LoadContent()
        {
            _logger.Debug("Loading Atlas Texture...");
            /*
            var sw = Stopwatch.StartNew();
            var atlasDataJson = File.ReadAllText(Path.Combine(contentManager.RootDirectory, $"{_atlasName}.json"));
            var atlasData = JsonConvert.DeserializeObject<Atlas>(atlasDataJson);
            _depth = atlasData.Depth;
            var atlasTextureData = File.ReadAllBytes(Path.Combine(contentManager.RootDirectory, $"{_atlasName}.blob"));
            AtlasTexture = new Texture3D(
                _graphicsDevice,
                atlasData.Width,
                atlasData.Height,
                atlasData.Depth,
                false,
                SurfaceFormat.Color);
            AtlasTexture.SetData(atlasTextureData);
            sw.Stop();
            _logger.Debug("Loading Atlas Texture...Done, Took {@Elapsed}s", sw.Elapsed.TotalSeconds);

            sw.Restart();
            _logger.Debug("Loading Atlas Data...");

            _landTiles = atlasData.Lands.ToDictionary(landTileData => landTileData.Id, landTileData => landTileData);
            _landTextureTiles = atlasData.LandTextures.ToDictionary(
                landTextureTileData => landTextureTileData.Id,
                landTextureTileData => landTextureTileData
            );
            _itemTiles = atlasData.Items.ToDictionary(staticTileData => staticTileData.Id, staticTileData => staticTileData);

            sw.Stop();
            _logger.Debug("Loading Atlas Data...Done. Took {@Elapsed}s", sw.Elapsed.TotalSeconds);
            */
        }
    }
}
