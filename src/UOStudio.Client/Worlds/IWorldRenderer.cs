using System;
using UOStudio.Client.Engine;
using UOStudio.Client.Engine.Graphics;

namespace UOStudio.Client.Worlds
{
    internal interface IWorldRenderer : IDisposable
    {
        void Draw(IGraphicsDevice graphicsDevice, World world, Camera camera);

        bool Load(IGraphicsDevice graphicsDevice);

        void Update(World world, Camera camera);

        void UpdateChunk(WorldChunk worldChunk);
    }
}
