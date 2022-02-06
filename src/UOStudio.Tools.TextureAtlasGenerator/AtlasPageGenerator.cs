using System.Collections.Generic;
using System.Drawing;
using Microsoft.Extensions.Configuration;
using Serilog;
using UOStudio.Tools.TextureAtlasGenerator.Abstractions;
using UOStudio.Tools.TextureAtlasGenerator.Contracts;

namespace UOStudio.Tools.TextureAtlasGenerator
{
    internal sealed class AtlasPageGenerator : IAtlasPageGenerator
    {
        private readonly ILogger _logger;
        private readonly IUvwCalculatorStrategy _uvwCalculatorStrategy;
        private readonly ITileContainer _tileContainer;
        private readonly int _atlasPageSize;
        private readonly bool _drawGrid;

        public AtlasPageGenerator(
            ILogger logger,
            IConfiguration configuration,
            IUvwCalculatorStrategy uvwCalculatorStrategy,
            ITileContainer tileContainer)
        {
            _logger = logger;
            _uvwCalculatorStrategy = uvwCalculatorStrategy;
            _tileContainer = tileContainer;

            _atlasPageSize = int.TryParse(configuration["AtlasPageSize"], out var atlasPageSize)
                ? atlasPageSize
                : 2048;
            _drawGrid = bool.TryParse(configuration["DrawGrid"], out var drawGrid) && drawGrid;
        }

        public IReadOnlyCollection<Bitmap> GeneratePages(IReadOnlyList<TextureAsset> textureAssets)
        {
            var alreadyProcessed = new List<string>(4096);
            var atlasPages = new List<Bitmap>(64);
            var currentPixelPositionX = 0;
            var currentPixelPositionY = 0;
            var atlasPageNumber = 0;

            Graphics atlasPageGraphics = null;
            var firstItemOnTheRow = -1;
            var drawCountPerPage = new Dictionary<int, int>();

            var atlasPage = new Bitmap(_atlasPageSize, _atlasPageSize);
            atlasPages.Add(atlasPage);
            atlasPageGraphics = Graphics.FromImage(atlasPage);

            for (var i = 0; i < textureAssets.Count; ++i)
            {
                var textureAsset = textureAssets[i];
                var textureAssetWidth = textureAsset.Bitmap.Width;
                var textureAssetHeight = textureAsset.Bitmap.Height;

                var tileUvws = _uvwCalculatorStrategy.CalculateUvws(
                    textureAsset,
                    _atlasPageSize,
                    currentPixelPositionX,
                    currentPixelPositionY,
                    atlasPageNumber);

                if (textureAsset.TileType == TileType.Item)
                {
                    _tileContainer.AddItemTile(new ItemTile(textureAsset, tileUvws));
                }
                else if (textureAsset.TileType == TileType.Land)
                {
                    _tileContainer.AddLandTile(new LandTile(textureAsset, tileUvws));
                }
                else
                {
                    _tileContainer.AddLandTextureTile(new LandTile(textureAsset, tileUvws));
                }

                if (!alreadyProcessed.Contains(textureAsset.ArtHash))
                {
                    if (currentPixelPositionX == 0)
                    {
                        firstItemOnTheRow = i;
                    }

                    atlasPageGraphics!.DrawImageUnscaled(
                        textureAsset.Bitmap,
                        currentPixelPositionX,
                        currentPixelPositionY);
                    if (_drawGrid)
                    {
                        atlasPageGraphics.DrawRectangle(
                            Pens.Fuchsia,
                            currentPixelPositionX,
                            currentPixelPositionY,
                            textureAssetWidth,
                            textureAssetHeight);
                    }

                    currentPixelPositionX += textureAssetWidth;
                    if (currentPixelPositionX + textureAssetWidth > _atlasPageSize)
                    {
                        currentPixelPositionX = 0;
                        currentPixelPositionY += textureAssets[firstItemOnTheRow].Bitmap.Height;
                    }
                    if (currentPixelPositionY + textureAssets[firstItemOnTheRow].Bitmap.Height >
                        _atlasPageSize)
                    {
                        currentPixelPositionX = 0;
                        currentPixelPositionY = 0;

                        atlasPageNumber++;
                        drawCountPerPage.Add(atlasPageNumber, 0);

                        //atlasPage?.Dispose();
                        atlasPage = new Bitmap(_atlasPageSize, _atlasPageSize);
                        atlasPages.Add(atlasPage);
                        atlasPageGraphics?.Dispose();
                        atlasPageGraphics = Graphics.FromImage(atlasPage);

                        continue;
                    }

                    alreadyProcessed.Add(textureAsset.ArtHash);
                }
            }

            atlasPageGraphics?.Dispose();

            return atlasPages;
        }
    }
}
