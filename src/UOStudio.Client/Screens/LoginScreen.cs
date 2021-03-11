using System.Drawing;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;
using UOStudio.Client.Core.Extensions;
using UOStudio.Client.UI;
using Color = Microsoft.Xna.Framework.Color;

namespace UOStudio.Client.Screens
{
    public class LoginScreen : GameScreen
    {
        private readonly ILogger _logger;
        private readonly IWindowProvider _windowProvider;
        private SpriteBatch _spriteBatch;
        private MouseState _mouseState;
        private ButtonState _mouseLeftButton;
        private PointF _mousePosition;

        private int _selectedThemeIndex;
        private int _selectedLanguage;

        public LoginScreen(
            Game game,
            ILogger logger,
            IWindowProvider windowProvider)
            : base(game)
        {
            _logger = logger;
            _windowProvider = windowProvider;
        }

        public override void Update(GameTime gameTime)
        {
            _mouseState = Mouse.GetState();
            _mousePosition = new PointF(_mouseState.X, _mouseState.Y);
            _mouseLeftButton = _mouseState.LeftButton;
        }

        public override void Draw(GameTime gameTime)
        {
            var r = new RectangleF(32, 32, 196, 32);
            var c = r.Contains(_mousePosition)
                ? _mouseLeftButton == ButtonState.Pressed
                    ? Color.Firebrick
                    : Color.Yellow
                : Color.Peru;
            var t = r.Contains(_mousePosition)
                ? 4.0f
                : 1.0f;

            _spriteBatch.Begin();
            _spriteBatch.DrawRectangle(r, c, t);
            _spriteBatch.End();
        }

        public override void DrawUserInterface(GameTime gameTime)
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("File"))
                    {
                        if (ImGui.MenuItem("Test"))
                        {
                            ScreenHandler.LoadScreen(new ProjectScreen(Game, _logger, @"D:\Private\Code\Projects\UOStudio\src\UOStudio.Client\bin\Debug\Projects\Temp"));
                        }
                        if (ImGui.MenuItem("Quit"))
                        {
                            ScreenHandler.Game.Exit();
                        }

                        ImGui.EndMenu();
                    }

                    var themes = GetThemes();
                    ImGui.SameLine(ImGui.GetWindowWidth() - 360);
                    ImGui.SetNextItemWidth(60);
                    ImGui.TextUnformatted("Theme");
                    ImGui.SameLine(ImGui.GetWindowWidth() - 300);
                    ImGui.SetNextItemWidth(112);
                    if (ImGui.Combo("##Theme", ref _selectedThemeIndex, themes, themes.Length))
                    {
                        if (_selectedThemeIndex == 0)
                        {
                            ImGui.StyleColorsLight();
                        }
                        else if (_selectedThemeIndex == 1)
                        {
                            ImGui.StyleColorsDark();
                        }
                    }

                    var languages = GetLanguages();
                    ImGui.SetNextItemWidth(60);
                    ImGui.SameLine(ImGui.GetWindowWidth() - 180);
                    ImGui.TextUnformatted("Language");
                    ImGui.SameLine(ImGui.GetWindowWidth() - 120);
                    ImGui.SetNextItemWidth(112);
                    if (ImGui.Combo("##Language", ref _selectedLanguage, languages, languages.Length))
                    {
                    }

                    ImGui.EndMenuBar();
                }

                ImGui.EndMainMenuBar();
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public override void Dispose()
        {
            _spriteBatch.Dispose();
            base.Dispose();
        }

        private static string[] GetThemes()
        {
            return new[]
            {
                "Light",
                "Dark",
                "Custom"
            };
        }

        private static string[] GetLanguages()
        {
            return new[]
            {
                "en-US"
            };
        }
    }
}
