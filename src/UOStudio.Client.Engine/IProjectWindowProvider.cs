using Microsoft.Xna.Framework.Content;
using UOStudio.Client.Engine.UI;

namespace UOStudio.Client.Engine
{
    public interface IProjectWindowProvider
    {
        void Draw();

        void LoadContent(ContentManager contentManager, ImGuiRenderer guiRenderer);

        void UnloadContent();
    }
}
