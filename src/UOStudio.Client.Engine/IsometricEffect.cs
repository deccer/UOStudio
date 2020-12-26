using System.Globalization;
using System.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UOStudio.Client.Engine
{
    public class IsometricEffect : MatrixEffect2
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Matrix _matrix = Matrix.Identity;
        private Vector2 _viewPort;

        public IsometricEffect(GraphicsDevice graphicsDevice, Effect effect)
            : base(effect)
        {
            _graphicsDevice = graphicsDevice;
            WorldMatrix = effect.Parameters[nameof(WorldMatrix)];
            Viewport = effect.Parameters[nameof(Viewport)];
            Brightlight = effect.Parameters[nameof(Brightlight)];

            effect.CurrentTechnique = effect.Techniques["HueTechnique"];

            effect.Name = "IsoMetricEffect";
        }

        public EffectParameter WorldMatrix { get; }
        public EffectParameter Viewport { get; }
        public EffectParameter Brightlight { get; }

        public override void ApplyStates(Matrix matrix)
        {
            WorldMatrix.SetValue(_matrix);

            _viewPort.X = _graphicsDevice.Viewport.Width;
            _viewPort.Y = _graphicsDevice.Viewport.Height;
            Viewport.SetValue(_viewPort);

            base.ApplyStates(matrix);
        }

        private static byte[] GetIsometricEffectData()
        {
            var rm = new ResourceManager("UOStudio.Client.Engine.Resources.Shaders", typeof(IsometricEffect).Assembly);
            var resource = rm.GetObject("IsometricEffect", CultureInfo.InvariantCulture);
            return (byte[])resource;
        }
    }
}
