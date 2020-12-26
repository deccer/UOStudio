using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UOStudio.Client.Engine
{
    public class MatrixEffect : Effect
    {
        protected MatrixEffect(GraphicsDevice graphicsDevice, byte[] effectCode)
            : base(graphicsDevice, effectCode)
        {
            MatrixTransform = Parameters[nameof(MatrixTransform)];
        }

        public EffectParameter MatrixTransform { get; }

        public virtual void ApplyStates(Matrix matrix)
        {
            MatrixTransform.SetValue(matrix);

            foreach (EffectPass effectPass in CurrentTechnique.Passes)
            {
                effectPass.Apply();
            }
        }
    }

    public class MatrixEffect2 : IDisposable
    {
        private readonly Effect _effect;

        public MatrixEffect2(Effect effect)
        {
            _effect = effect;
            MatrixTransform = effect.Parameters[nameof(MatrixTransform)];
        }

        public EffectParameter MatrixTransform { get; }

        public virtual void ApplyStates(Matrix matrix)
        {
            MatrixTransform.SetValue(matrix);

            foreach (EffectPass effectPass in _effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
            }
        }

        public void Dispose()
        {
            _effect?.Dispose();
        }
    }
}
