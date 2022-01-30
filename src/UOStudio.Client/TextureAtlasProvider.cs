using Microsoft.Extensions.Options;
using Serilog;
using UOStudio.Client.Engine.Graphics;

namespace UOStudio.Client
{
    internal sealed class TextureAtlasProvider : ITextureAtlasProvider
    {
        private readonly ILogger _logger;
        private readonly IOptions<ClientSettings> _clientSettings;
        private readonly IGraphicsDevice _graphicsDevice;

        public TextureAtlasProvider(
            ILogger logger,
            IOptions<ClientSettings> clientSettings,
            IGraphicsDevice graphicsDevice)
        {
            _logger = logger;
            _clientSettings = clientSettings;
            _graphicsDevice = graphicsDevice;
        }

        public bool TryLoadTextureAtlas(string atlasName, out ITextureAtlas textureAtlas)
        {
            textureAtlas = new TextureAtlas(
                _logger,
                _graphicsDevice,
                _clientSettings.Value.ProjectsDirectory,
                atlasName);
            if (textureAtlas.Load())
            {
                return true;
            }

            textureAtlas = null;
            return false;
        }
    }
}
