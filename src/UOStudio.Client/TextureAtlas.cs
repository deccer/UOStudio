using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Serilog;
using UOStudio.Client.Engine.Graphics;
using UOStudio.Tools.TextureAtlasGenerator.Contracts;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace UOStudio.Client
{
    internal sealed class TextureAtlas : ITextureAtlas
    {
        private readonly LandTile _invalidLandTile;
        private readonly ItemTile _invalidItemTile;

        private readonly ILogger _logger;
        private readonly IGraphicsDevice _graphicsDevice;
        private readonly TextureFormat _textureFormat;
        private readonly string _projectsDirectory;
        private readonly string _atlasName;

        private IDictionary<int, LandTile> _landTiles;
        private IDictionary<int, LandTile> _landTextureTiles;
        private IDictionary<int, ItemTile> _itemTiles;
        private int _depth;

        public int Depth => _depth;

        public ITextureArray AtlasTexture { get; private set; }

        public ITextureView[] AtlasTextureViews { get; private set; }

        internal TextureAtlas(
            ILogger logger,
            IGraphicsDevice graphicsDevice,
            TextureFormat textureFormat,
            string projectsDirectory,
            string atlasName)
        {
            _logger = logger.ForContext<TextureAtlas>();
            _graphicsDevice = graphicsDevice;
            _textureFormat = textureFormat;
            _projectsDirectory = projectsDirectory;
            _atlasName = atlasName;
            _invalidLandTile = new LandTile(default, default);
            _invalidItemTile = new ItemTile(default, default);

            _landTiles = new Dictionary<int, LandTile>();
            _landTextureTiles = new Dictionary<int, LandTile>();
            _itemTiles = new Dictionary<int, ItemTile>();
        }

        public void Dispose()
        {
            foreach (var atlastTextureViews in AtlasTextureViews)
            {
                atlastTextureViews?.Dispose();
            }
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

        public int LandTileCount => _landTiles.Count;

        public int LandTextureCount => _landTextureTiles.Count;

        public int ItemTileCount => _itemTiles.Count;

        public bool Load()
        {
            _logger.Debug("Loading Atlas Texture...");

            var atlasJsonFilePath = Path.Combine(_projectsDirectory, $"{_atlasName}.json");
            var atlasTextureDataFilePath = Path.Combine(_projectsDirectory, $"{_atlasName}.blob");
            if (!File.Exists(atlasJsonFilePath))
            {
                _logger.Error("TextureAtlas - Metadata file {@AtlasJson} cannot be found", atlasJsonFilePath);
                return false;
            }

            if (!File.Exists(atlasTextureDataFilePath))
            {
                _logger.Error("TextureAtlas - TextureData file {@TextureData} cannot be found", atlasTextureDataFilePath);
                return false;
            }

            var sw = Stopwatch.StartNew();
            var atlasDataJson = File.ReadAllText(atlasJsonFilePath);
            var atlasData = JsonConvert.DeserializeObject<Atlas>(atlasDataJson);
            if (atlasData == null)
            {
                _logger.Error("TextureAtlas - Metadata incorrect");
                sw.Stop();
                return false;
            }

            _depth = atlasData.Depth;
            var atlasTextureData = File.ReadAllBytes(atlasTextureDataFilePath);
            AtlasTexture = _graphicsDevice.CreateTextureArrayFromBytes(
                atlasData.Width,
                atlasData.Height,
                atlasData.Depth,
                atlasTextureData,
                _textureFormat);
            AtlasTextureViews = new ITextureView[atlasData.Depth];
            for (uint i = 0; i < Depth; i++)
            {
                AtlasTextureViews[i] = new TextureView(AtlasTexture, i);
            }

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
            return true;
        }
    }
}
