using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UOStudio.TextureAtlasGenerator.Abstractions;

namespace UOStudio.TextureAtlasGenerator
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var serviceProvider = BuildServiceProvider();
            var textureAtlasGenerator = serviceProvider.GetService<IAtlasGenerator>();

            textureAtlasGenerator!.Run();
        }

        private static IServiceProvider BuildServiceProvider()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton(Log.Logger);

            services.AddSingleton<IHashCalculator, Sha1HashCalculator>();
            services.AddSingleton<IAssetExtractor, AssetExtractor>();
            services.AddSingleton<IAssetSorter, AssetSorter>();
            services.AddSingleton<IAtlasPageGenerator, AtlasPageGenerator>();
            services.AddSingleton<IUvwCalculator, ItemUvwCalculator>();
            services.AddSingleton<IUvwCalculator, LandUvwCalculator>();
            services.AddSingleton<IUvwCalculator, LandTextureUvwCalculator>();
            services.AddSingleton<IUvwCalculatorStrategy, UvwCalculatorStrategy>();
            services.AddSingleton<IAtlasGenerator, AtlasGenerator>();
            services.AddSingleton<ITileContainer, TileContainer>();
            services.AddSingleton<ITexture3dGenerator, Texture3dGenerator>();
            services.AddSingleton<IUltimaArtProvider, UltimaArtProvider>();
            return services.BuildServiceProvider();
        }
    }
}
