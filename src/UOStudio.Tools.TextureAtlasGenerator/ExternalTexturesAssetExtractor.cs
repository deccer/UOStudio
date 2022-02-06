using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Serilog;
using UOStudio.Tools.TextureAtlasGenerator.Abstractions;
using UOStudio.Tools.TextureAtlasGenerator.Contracts;

namespace UOStudio.Tools.TextureAtlasGenerator
{
    internal sealed class ExternalTexturesAssetExtractor : IAssetExtractor
    {
        private readonly ILogger _logger;
        private readonly IHashCalculator _hashCalculator;
        private readonly string _externalTexturesPath;

        public ExternalTexturesAssetExtractor(
            ILogger logger,
            IHashCalculator hashCalculator,
            IConfiguration configuration)
        {
            _logger = logger;
            _hashCalculator = hashCalculator;
            _externalTexturesPath = configuration["ExternalTexturesPath"];
        }

        public IReadOnlyCollection<TextureAsset> ExtractAssets()
        {
            _logger.Information("Extracting textures from {@ExternalTexturesPath}", _externalTexturesPath);
            return Directory
                .EnumerateFiles(_externalTexturesPath, "*.png", SearchOption.AllDirectories)
                .Select((filePath, index) =>
                    {
                        var fileBytes = File.ReadAllBytes(filePath);
                        var fileHash = _hashCalculator.CalculateHash(fileBytes);
                        var bitmap = Bitmap.FromFile(filePath);
                        return new TextureAsset(index, TileType.LandTexture, fileHash, (Bitmap)bitmap);
                    }
                )
                .ToList();
        }
    }
}
