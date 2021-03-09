using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UOStudio.Client.Core.Extensions;
using Color = Microsoft.Xna.Framework.Color;

namespace UOStudio.Client.Screens.Transitions
{
    public class FadeTransition : Transition
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;

        public FadeTransition(GraphicsDevice graphicsDevice, Color color, float duration = 1.0f)
            : base(duration)
        {
            Color = color;

            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public override void Dispose()
        {
            _spriteBatch.Dispose();
        }

        public Color Color { get; }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            var bounds = _graphicsDevice.Viewport.Bounds;
            _spriteBatch.FillRectangle(new RectangleF(bounds.Left, bounds.Top, bounds.Width, bounds.Height), Color * Value);
            _spriteBatch.End();
        }
    }
}
