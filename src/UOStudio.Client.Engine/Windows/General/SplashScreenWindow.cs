using System;
using ImGuiNET;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UOStudio.Client.Engine.UI;
using Num = System.Numerics;

namespace UOStudio.Client.Engine.Windows.General
{
    public sealed class SplashScreenWindow : Window
    {
        private Texture2D _splashScreenTexture;
        private IntPtr _splashScreenTextureId;

        public SplashScreenWindow(IFileVersionProvider fileVersionProvider)
            : base($"UO Studio {fileVersionProvider.GetVersion()}")
        {
            Show();
        }

        public override void UnloadContent()
        {
            _splashScreenTexture?.Dispose();
            base.UnloadContent();
        }

        protected override void LoadContentInternal(
            GraphicsDevice graphicsDevice,
            ContentManager contentManager,
            ImGuiRenderer imGuiRenderer)
        {
            _splashScreenTexture = contentManager.Load<Texture2D>("Content/splashscreen");
            _splashScreenTextureId = imGuiRenderer.BindTexture(_splashScreenTexture);

            base.LoadContentInternal(graphicsDevice, contentManager, imGuiRenderer);
        }

        protected override void DrawInternal()
        {
            var backBufferSize = ImGui.GetWindowViewport().Size;
            ImGui.SetWindowPos(new Num.Vector2(backBufferSize.X / 2.0f - _splashScreenTexture.Width / 2.0f, backBufferSize.Y / 2.0f - _splashScreenTexture.Height / 2.0f));
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Num.Vector2(0, 0));
            if (ImGui.ImageButton(_splashScreenTextureId, new Num.Vector2(_splashScreenTexture.Width, _splashScreenTexture.Height), Num.Vector2.Zero, Num.Vector2.One, 0))
            {
                ToggleVisibility();
            }
            ImGui.PopStyleVar();
        }
    }
}
