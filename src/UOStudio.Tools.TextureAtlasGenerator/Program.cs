using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UOStudio.Client.Engine.Extensions;
using UOStudio.Tools.TextureAtlasGenerator.Abstractions;

namespace UOStudio.Tools.TextureAtlasGenerator
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

            services.AddEngineKit(configuration);
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
            services.AddSingleton<ITextureArrayGenerator, TextureArrayGenerator>();
            services.AddSingleton<IUltimaArtProvider, UltimaArtProvider>();
            return services.BuildServiceProvider();
        }
    }
}
