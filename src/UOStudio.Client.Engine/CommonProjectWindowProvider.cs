using Microsoft.Xna.Framework.Content;
using UOStudio.Client.Core.Settings;
using UOStudio.Client.Engine.UI;
using UOStudio.Client.Engine.Windows;
using UOStudio.Client.Engine.Windows.General;

namespace UOStudio.Client.Engine
{
    public abstract class CommonProjectWindowProvider : IProjectWindowProvider
    {
        protected CommonProjectWindowProvider(IAppSettingsProvider appSettingsProvider, IFileVersionProvider fileVersionProvider)
        {
            SplashScreenWindow = new SplashScreenWindow(fileVersionProvider);
            AboutWindow = new AboutWindow(fileVersionProvider);
            FrameTimeOverlayWindow = new FrameTimeOverlayWindow("Frame Times");
            Settings = new SettingsWindow(appSettingsProvider);
        }

        public AboutWindow AboutWindow { get; }

        public SplashScreenWindow SplashScreenWindow { get; }

        public FrameTimeOverlayWindow FrameTimeOverlayWindow { get; }

        public SettingsWindow Settings { get; }

        public virtual void Draw()
        {
            SplashScreenWindow.Draw();
            AboutWindow.Draw();
            FrameTimeOverlayWindow.Draw();
            Settings.Draw();
        }

        public virtual void LoadContent(ContentManager contentManager, ImGuiRenderer guiRenderer)
        {
            SplashScreenWindow.LoadContent(contentManager, guiRenderer);
            AboutWindow.LoadContent(contentManager, guiRenderer);
            FrameTimeOverlayWindow.LoadContent(contentManager, guiRenderer);
            Settings.LoadContent(contentManager, guiRenderer);
        }

        public virtual void UnloadContent()
        {
            SplashScreenWindow.UnloadContent();
            AboutWindow.UnloadContent();
            FrameTimeOverlayWindow.UnloadContent();
            Settings.UnloadContent();
        }
    }
}
