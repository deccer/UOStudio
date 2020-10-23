using Microsoft.Xna.Framework.Graphics;

namespace UOStudio.Client.Engine.Ultima
{
    public class UltimaTexture : Texture2D
    {
        public UltimaTexture(GraphicsDevice graphicsDevice, int width, int height)
            : base(graphicsDevice, width, height, false, SurfaceFormat.Color)
        {
            Ticks = 300000;
        }

        public long Ticks { get; set; }

        public uint[] Data { get; private set; }

        public void ApplyData(uint[] data)
        {
            Data = data;
            SetData(data);
        }
    }
}
