using System;
using System.Numerics;
using ImGuiNET;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UOStudio.Client.Engine.UI;

namespace UOStudio.Client.Engine.Windows.MapEdit
{
    public class MapToolbarWindow : Window
    {
        private ToolDescription[] _toolDescriptions;

        private Texture2D _mapControlLogin;
        private Texture2D _mapControlLogout;

        private Texture2D _mapToolTerrainElevate;
        private Texture2D _mapToolTerrainLower;
        private Texture2D _mapToolTerrainAdd;
        private Texture2D _mapToolTerrainSubtract;
        private Texture2D _mapToolTerrainCoast;
        private Texture2D _mapToolTerrainFlatten;
        private Texture2D _mapToolTerrainPaint;
        private Texture2D _mapToolTerrainSmooth;

        public MapToolbarWindow()
            : base("Controls")
        {
        }

        public event Action LoginClicked;
        public event Action LogoutClicked;
        public event Action TerrainAddClicked;
        public event Action TerrainRemoveClicked;

        public int IconSize { get; set; } = 32;

        public override void UnloadContent()
        {
            _mapControlLogin.Dispose();
            _mapControlLogout.Dispose();
            _mapToolTerrainAdd.Dispose();
            _mapToolTerrainCoast.Dispose();
            _mapToolTerrainElevate.Dispose();
            _mapToolTerrainFlatten.Dispose();
            _mapToolTerrainLower.Dispose();
            _mapToolTerrainPaint.Dispose();
            _mapToolTerrainSmooth.Dispose();
            _mapToolTerrainSubtract.Dispose();
        }

        protected override void LoadContentInternal(GraphicsDevice graphicsDevice, ContentManager contentManager, ImGuiRenderer guiRenderer)
        {
            _mapControlLogin = contentManager.Load<Texture2D>($"Content/Controls/control-login-{IconSize}");
            _mapControlLogout = contentManager.Load<Texture2D>($"Content/Controls/control-logout-{IconSize}");

            _mapToolTerrainElevate = contentManager.Load<Texture2D>($"Content/Tools/terrain-elevate-{IconSize}");
            _mapToolTerrainLower = contentManager.Load<Texture2D>($"Content/Tools/terrain-lower-{IconSize}");
            _mapToolTerrainAdd = contentManager.Load<Texture2D>($"Content/Tools/terrain-add-{IconSize}");
            _mapToolTerrainSubtract = contentManager.Load<Texture2D>($"Content/Tools/terrain-subtract-{IconSize}");
            _mapToolTerrainCoast = contentManager.Load<Texture2D>($"Content/Tools/terrain-coast-{IconSize}");
            _mapToolTerrainFlatten = contentManager.Load<Texture2D>($"Content/Tools/terrain-flatten-{IconSize}");
            _mapToolTerrainPaint = contentManager.Load<Texture2D>($"Content/Tools/terrain-paint-{IconSize}");
            _mapToolTerrainSmooth = contentManager.Load<Texture2D>($"Content/Tools/terrain-smooth-{IconSize}");

            var mapControlLogin = guiRenderer.BindTexture(_mapControlLogin);
            var mapControlLogout = guiRenderer.BindTexture(_mapControlLogout);

            var mapToolTerrainElevate = guiRenderer.BindTexture(_mapToolTerrainElevate);
            var mapToolTerrainLower = guiRenderer.BindTexture(_mapToolTerrainLower);
            var mapToolTerrainAdd = guiRenderer.BindTexture(_mapToolTerrainAdd);
            var mapToolTerrainSubtract = guiRenderer.BindTexture(_mapToolTerrainSubtract);
            var mapToolTerrainCoast = guiRenderer.BindTexture(_mapToolTerrainCoast);
            var mapToolTerrainFlatten = guiRenderer.BindTexture(_mapToolTerrainFlatten);
            var mapToolTerrainPaint = guiRenderer.BindTexture(_mapToolTerrainPaint);
            var mapToolTerrainSmooth = guiRenderer.BindTexture(_mapToolTerrainSmooth);

            var loginToolDescription = new ToolDescription
            {
                Group = ToolGroup.Control,
                Name = "Login",
                TextureHandle = mapControlLogin,
                Size = IconSize
            };
            loginToolDescription.Clicked += LoginClicked;

            var logoutToolDescription = new ToolDescription
            {
                Group = ToolGroup.Control,
                Name = "Logout", TextureHandle = mapControlLogout,
                Size = IconSize
            };
            logoutToolDescription.Clicked += LogoutClicked;

            _toolDescriptions = new[]
            {
                loginToolDescription,
                logoutToolDescription,
                new ToolDescription { Group = ToolGroup.Selection, Name = "Elevate", TextureHandle = mapToolTerrainElevate, Size = IconSize },
                new ToolDescription { Group = ToolGroup.Selection, Name = "Lower", TextureHandle = mapToolTerrainLower, Size = IconSize },
                new ToolDescription { Group = ToolGroup.Selection, Name = "Add", TextureHandle = mapToolTerrainAdd, Size = IconSize },
                new ToolDescription { Group = ToolGroup.Selection, Name = "Subtract", TextureHandle = mapToolTerrainSubtract, Size = IconSize },
                new ToolDescription { Group = ToolGroup.Selection, Name = "Coast", TextureHandle = mapToolTerrainCoast, Size = IconSize },
                new ToolDescription { Group = ToolGroup.Selection, Name = "Flatten", TextureHandle = mapToolTerrainFlatten, Size = IconSize },
                new ToolDescription { Group = ToolGroup.Selection, Name = "Paint", TextureHandle = mapToolTerrainPaint, Size = IconSize },
                new ToolDescription { Group = ToolGroup.Selection, Name = "Smooth", TextureHandle = mapToolTerrainSmooth, Size = IconSize }
            };
        }

        protected override void DrawInternal()
        {
            ImGui.PopStyleVar(1);

            var currentToolGroup = ToolGroup.Control;
            foreach (var toolDescription in _toolDescriptions)
            {
                if (currentToolGroup != toolDescription.Group)
                {
                    ImGui.Dummy(new Vector2(4, toolDescription.Size));
                    ImGui.SameLine();
                }
                currentToolGroup = toolDescription.Group;

                if (ImGui.ImageButton(toolDescription.TextureHandle, new Vector2(toolDescription.Size, toolDescription.Size)))
                {
                    toolDescription.Click();
                }
                ImGui.SameLine();
            }
        }

        protected override ImGuiWindowFlags GetWindowFlags()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            return base.GetWindowFlags();
        }
    }
}
