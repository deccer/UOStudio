using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using UOStudio.Client.Core.Settings;
using UOStudio.Client.Engine.Ultima;

namespace UOStudio.Client.Engine
{
    public sealed class UltimaProvider : IUltimaProvider
    {
        private readonly IAppSettingsProvider _appSettingsProvider;

        public UltimaProvider(IAppSettingsProvider appSettingsProvider)
        {
            _appSettingsProvider = appSettingsProvider;
        }

        public UltimaArtLoader ArtLoader { get; private set; }

        public Task Load(ILogger logger, GraphicsDevice graphicsDevice)
        {
            ArtLoader = new UltimaArtLoader(graphicsDevice, _appSettingsProvider.AppSettings.General.UltimaOnlineBasePath, UltimaConstants.MaxStaticDataIndexCount, UltimaConstants.MaxLandDataIndexCount, false);

            return Task.WhenAll(ArtLoader.Load(logger));
        }
    }
}
