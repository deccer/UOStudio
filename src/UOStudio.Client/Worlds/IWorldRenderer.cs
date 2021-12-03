using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace UOStudio.Client.Worlds
{
    internal interface IWorldRenderer : IDisposable
    {
        void Draw(GraphicsDevice graphicsDevice, World world, Camera camera);

        void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice);

        void Update(World world, Camera camera);

        void UpdateChunk(WorldChunk worldChunk);
    }
}
