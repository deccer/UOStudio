using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UOStudio.Client.Engine.Ultima
{
    public class UltimaArtTexture : UltimaTexture
    {
        public UltimaArtTexture(GraphicsDevice graphicsDevice, int width, int height)
            : base(graphicsDevice, width, height)
        {
        }

        public Rectangle ImageRectangle { get; set; }
    }
}
