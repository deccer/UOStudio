using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace UOStudio.Client.Engine
{
    public interface IItemProvider
    {
        int Length { get; }

        unsafe Texture GetLand(GraphicsDevice graphicsDevice, int index);

        Task<Texture> GetLandAsync(GraphicsDevice graphicsDevice, int index);

        unsafe Texture2D GetStatic(GraphicsDevice graphicsDevice, int index);

        Task<Texture2D> GetStaticAsync(GraphicsDevice graphicsDevice, int index);
    }
}
