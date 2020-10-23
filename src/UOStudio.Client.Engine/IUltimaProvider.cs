using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using UOStudio.Client.Engine.Ultima;

namespace UOStudio.Client.Engine
{
    public interface IUltimaProvider
    {
        UltimaArtLoader ArtLoader { get; }

        Task Load(ILogger logger, GraphicsDevice graphicsDevice);
    }
}
