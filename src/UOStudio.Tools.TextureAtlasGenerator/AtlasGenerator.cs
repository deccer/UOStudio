using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Serilog;
using UOStudio.TextureAtlasGenerator.Abstractions;

namespace UOStudio.TextureAtlasGenerator
{
    internal sealed class AtlasGenerator : IAtlasGenerator
    {
        private readonly ILogger _logger;
        private readonly IAssetExtractor _assetExtractor;
        private readonly IAssetSorter _assetSorter;
        private readonly IAtlasPageGenerator _atlasPageGenerator;
        private readonly ITileContainer _tileContainer;
        private readonly ITexture3dGenerator _texture3dGenerator;

        private readonly string _exportPath;
        private readonly bool _storeIndividualPages;

        public AtlasGenerator(
            ILogger logger,
            IConfiguration configuration,
            IAssetExtractor assetExtractor,
            IAssetSorter assetSorter,
            IAtlasPageGenerator atlasPageGenerator,
            ITileContainer tileContainer,
            ITexture3dGenerator texture3dGenerator)
        {
            _logger = logger.ForContext<AtlasGenerator>();
            _assetExtractor = assetExtractor;
            _assetSorter = assetSorter;
            _atlasPageGenerator = atlasPageGenerator;
            _tileContainer = tileContainer;
            _texture3dGenerator = texture3dGenerator;

            _exportPath = configuration["ExportPath"];
            _storeIndividualPages =
                bool.TryParse(configuration["StoreIndividualPages"], out var storeIndividualPages) && storeIndividualPages;
        }

        public void Run()
        {
            if (!Directory.Exists(_exportPath))
            {
                Directory.CreateDirectory(_exportPath);
            }

            var textureAssets = _assetExtractor.ExtractAssets();
            _logger.Information("Extracted {@Count} Assets", textureAssets.Count);

            textureAssets = _assetSorter.SortAssets(textureAssets);

            var atlasPages = _atlasPageGenerator.GeneratePages(textureAssets.ToList());

            if (_storeIndividualPages)
            {
                var atlasPageNumber = 0;
                var guid = Guid.NewGuid().ToString();
                foreach (var atlasPage in atlasPages)
                {
                    var fileName = Path.Combine(_exportPath, $"{guid}-{atlasPageNumber:00}.png");
                    atlasPage.Save(fileName);
                    atlasPageNumber += 1;
                }
            }

            var texture3dData = _texture3dGenerator.Generate3dTexture(atlasPages);
            File.WriteAllBytes(Path.Combine(_exportPath, "Atlas.blob"), texture3dData);
            _tileContainer.Save(Path.Combine(_exportPath, "Atlas.json"), atlasPages.Count);
            _logger.Information("Done");
        }
    }
}
