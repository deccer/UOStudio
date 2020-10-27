using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UOStudio.Client.Engine.UI;

namespace UOStudio.Client.Engine
{
    public interface IProjectWindowProvider
    {
        void Draw();

        void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager, ImGuiRenderer guiRenderer);

        void UnloadContent();
    }
}
