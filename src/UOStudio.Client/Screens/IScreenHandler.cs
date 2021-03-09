using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using UOStudio.Client.Screens.Transitions;
using UOStudio.Client.UI;

namespace UOStudio.Client.Screens
{
    public interface IScreenHandler
    {
        void LoadScreen(Screen screen, Transition transition);

        void LoadScreen(Screen screen);

        void Initialize(Game game);

        void LoadContent(ContentManager contentManager);

        void UnloadContent();

        void Update(GameTime gameTime);

        void Draw(GameTime gameTime, ImGuiRenderer imGuiRenderer);
    }
}
