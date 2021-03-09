using System;
using Microsoft.Xna.Framework;

namespace UOStudio.Client.Screens
{
    public abstract class Screen : IDisposable
    {
        public ScreenHandler ScreenHandler { get; internal set; }

        public virtual void Dispose() { }

        public virtual void Initialize() { }

        public virtual void LoadContent() { }

        public virtual void UnloadContent() { }

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);

        public virtual void DrawUserInterface(GameTime gameTime) { }
    }
}
