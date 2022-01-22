using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using UOStudio.TextureAtlasGenerator.Abstractions;
using UOStudio.TextureAtlasGenerator.Contracts;

namespace UOStudio.TextureAtlasGenerator
{
    internal sealed class UvwCalculatorStrategy : IUvwCalculatorStrategy
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IUvwCalculator> _uvwCalculators;

        public UvwCalculatorStrategy(
            ILogger logger,
            IEnumerable<IUvwCalculator> uvwCalculators)
        {
            _logger = logger.ForContext<UvwCalculatorStrategy>();
            _uvwCalculators = uvwCalculators;
        }

        public Uvws CalculateUvws(TextureAsset textureAsset, int atlasPageSize, int currentX, int currentY, int page)
        {
            var uvwCalculator = _uvwCalculators.FirstOrDefault(c => c.TileType == textureAsset.TileType);
            if (uvwCalculator == null)
            {
                _logger.Error("Unable to find a uvw calculator for tile type {@TileType}", textureAsset.TileType);
                throw new InvalidOperationException();
            }

            return uvwCalculator.CalculateUvws(textureAsset, atlasPageSize, currentX, currentY, page);
        }
    }
}
