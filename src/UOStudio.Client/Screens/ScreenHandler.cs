using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Serilog;
using UOStudio.Client.Screens.Transitions;
using UOStudio.Client.UI;
using Num = System.Numerics;
namespace UOStudio.Client.Screens
{
    public sealed class ScreenHandler : IScreenHandler
    {
        private readonly IWindowProvider _windowProvider;
        private readonly ILogger _logger;
        private Screen _activeScreen;
        private bool _isInitialized;
        private bool _isLoaded;
        private Transition _activeTransition;
        private Game _game;

        private Num.Vector3 _colorForText = new(236f / 255f, 240f / 255f, 241f / 255f);
        private Num.Vector3 _colorForHead = new(41f / 255f, 128f / 255f, 185f / 255f);
        private Num.Vector3 _colorForArea = new(57f / 255f, 79f / 255f, 105f / 255f);
        private Num.Vector3 _colorForBody = new(44f / 255f, 62f / 255f, 80f / 255f);
        private Num.Vector3 _colorForPops = new(33f / 255f, 46f / 255f, 60f / 255f);

        public ScreenHandler(
            ILogger logger,
            IWindowProvider windowProvider)
        {
            _windowProvider = windowProvider;
            _logger = logger.ForContext<ScreenHandler>();
        }

        public Game Game => _game;

        public void LoadScreen(Screen screen, Transition transition)
        {
            if (_activeTransition != null)
            {
                return;
            }

            _activeTransition = transition;
            _activeTransition.StateChanged += (_, _) => LoadScreen(screen);
            _activeTransition.Completed += (_, _) =>
            {
                _activeTransition.Dispose();
                _activeTransition = null;
            };
        }

        public void LoadScreen(Screen screen)
        {
            _activeScreen?.UnloadContent();
            _activeScreen?.Dispose();

            screen.ScreenHandler = this;

            screen.Initialize();

            screen.LoadContent();

            _activeScreen = screen;
        }

        public void Initialize(Game game)
        {
            _game = game;
            _activeScreen?.Initialize();
            _isInitialized = true;
        }

        public void LoadContent(ContentManager contentManager)
        {
            _activeScreen?.LoadContent();
            _isLoaded = true;
        }

        public void UnloadContent()
        {
            _activeScreen?.UnloadContent();
            _isLoaded = false;
        }

        public void Update(GameTime gameTime)
        {
            _activeScreen?.Update(gameTime);
            _activeTransition?.Update(gameTime);
        }

        public void Draw(GameTime gameTime, ImGuiRenderer imGuiRenderer)
        {
            _activeScreen?.Draw(gameTime);
            _activeTransition?.Draw(gameTime);

            imGuiRenderer.BeginLayout(gameTime);
            _activeScreen?.DrawUserInterface(gameTime);

            ImGui.ShowDemoWindow();

            _windowProvider.Draw();
            imGuiRenderer.EndLayout();
        }
    }
}
