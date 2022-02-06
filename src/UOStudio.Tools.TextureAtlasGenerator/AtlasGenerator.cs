using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Serilog;
using UOStudio.Tools.TextureAtlasGenerator.Abstractions;

namespace UOStudio.Tools.TextureAtlasGenerator
{
    internal sealed class AtlasGenerator : IAtlasGenerator
    {
        private readonly ILogger _logger;
        private readonly IAssetExtractor _assetExtractor;
        private readonly IAssetSorter _assetSorter;
        private readonly IAtlasPageGenerator _atlasPageGenerator;
        private readonly ITileContainer _tileContainer;
        private readonly ITextureArrayGenerator _textureArrayGenerator;

        private readonly string _exportPath;
        private readonly bool _storeIndividualPages;

        public AtlasGenerator(
            ILogger logger,
            IConfiguration configuration,
            IAssetExtractor assetExtractor,
            IAssetSorter assetSorter,
            IAtlasPageGenerator atlasPageGenerator,
            ITileContainer tileContainer,
            ITextureArrayGenerator textureArrayGenerator)
        {
            _logger = logger.ForContext<AtlasGenerator>();
            _assetExtractor = assetExtractor;
            _assetSorter = assetSorter;
            _atlasPageGenerator = atlasPageGenerator;
            _tileContainer = tileContainer;
            _textureArrayGenerator = textureArrayGenerator;

            _exportPath = configuration["ExportPath"];
            _storeIndividualPages =
                bool.TryParse(configuration["StoreIndividualPages"], out var storeIndividualPages) && storeIndividualPages;
        }

        public void Run(string atlasName)
        {
            if (!Directory.Exists(_exportPath))
            {
                Directory.CreateDirectory(_exportPath);
            }

            var textureAssets = _assetExtractor.ExtractAssets();
            _logger.Information("Extracted {@Count} Assets", textureAssets.Count);

            var sw = Stopwatch.StartNew();
            textureAssets = _assetSorter.SortAssets(textureAssets);
            sw.Stop();
            _logger.Information("Sorting art took {@Duration}s", sw.Elapsed.TotalSeconds);

            var atlasPages = _atlasPageGenerator.GeneratePages(textureAssets.ToList());

            if (_storeIndividualPages)
            {
                _logger.Information("Storing individual pages...");
                sw.Restart();
                var atlasPageNumber = 0;
                var guid = Guid.NewGuid().ToString();
                foreach (var atlasPage in atlasPages)
                {
                    var fileName = Path.Combine(_exportPath, $"{guid}-{atlasPageNumber:00}.png");
                    atlasPage.Save(fileName);
                    atlasPageNumber += 1;
                }
                sw.Stop();
                _logger.Information("Storing individual pages. Took {@Duration}s", sw.Elapsed.TotalSeconds);
            }

            var textureArrayBytes = _textureArrayGenerator.GenerateTextureArray(atlasPages);
            File.WriteAllBytes(Path.Combine(_exportPath, $"{atlasName}.blob"), textureArrayBytes);
            _tileContainer.Save(Path.Combine(_exportPath, $"{atlasName}.json"), atlasPages.Count);
            _logger.Information("Done");
        }
    }
}
