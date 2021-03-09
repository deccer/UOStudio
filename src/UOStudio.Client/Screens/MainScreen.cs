using ImGuiNET;
using Microsoft.Xna.Framework;
using Serilog;

namespace UOStudio.Client.Screens
{
    public class MainScreen : GameScreen
    {
        private readonly ILogger _logger;
        private readonly string _projectDirectory;
        private Map _map;
        private Camera _camera;

        public MainScreen(Game game, ILogger logger, string projectDirectory)
            : base(game)
        {
            _logger = logger.ForContext<MainScreen>();
            _projectDirectory = projectDirectory;
            _map = new Map(logger);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            _camera = new Camera(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight)
            {
                Mode = CameraMode.Perspective
            };
            _map.LoadContent(Content, GraphicsDevice, _projectDirectory, "Atlas");
        }

        public override void Update(GameTime gameTime)
        {
            _camera.Update(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
        }

        public override void Draw(GameTime gameTime)
        {
            _map.Draw(GraphicsDevice, _camera);
        }

        public override void DrawUserInterface(GameTime gameTime)
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("Editor"))
                    {
                        if (ImGui.MenuItem("Quit"))
                        {
                            ImGui.EndMenu();
                        }
                        ImGui.EndMenu();
                    }
                    ImGui.EndMenuBar();
                }
                ImGui.EndMainMenuBar();
            }
        }

        public override void Dispose()
        {
            _map.Dispose();
            base.Dispose();
        }
    }
}
