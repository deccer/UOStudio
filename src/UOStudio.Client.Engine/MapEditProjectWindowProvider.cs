using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UOStudio.Client.Core;
using UOStudio.Client.Core.Settings;
using UOStudio.Client.Engine.UI;
using UOStudio.Client.Engine.Windows.General;
using UOStudio.Client.Engine.Windows.MapEdit;

namespace UOStudio.Client.Engine.Windows
{
    public sealed class MapEditProjectWindowProvider : CommonProjectWindowProvider
    {
        private readonly ProfileService _profileService;
        private readonly IItemProvider _itemProvider;
        private readonly TileDataProvider _tileDataProvider;
        private readonly MapEditState _mapEditState;
        private readonly RenderTarget2D _mapEditRenderTarget;

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

        public MapEditProjectWindowProvider(
            IAppSettingsProvider appSettingsProvider,
            IFileVersionProvider fileVersionProvider,
            ProfileService profileService,
            IItemProvider itemProvider,
            TileDataProvider tileDataProvider,
            MapEditState mapEditState,
            RenderTarget2D mapEditRenderTarget)
            : base(appSettingsProvider, fileVersionProvider)
        {
            DockSpaceWindow = new DockSpaceWindow("MapEditDockspace");
            _profileService = profileService;
            _itemProvider = itemProvider;
            _tileDataProvider = tileDataProvider;
            _mapEditState = mapEditState;
            _mapEditRenderTarget = mapEditRenderTarget;
        }

        public MapToolbarWindow MapToolbarWindow { get; private set; }

        public MapViewWindow MapViewWindow { get; private set; }

        public MapItemBrowserWindow MapItemBrowserWindow { get; private set; }

        public MapLandBrowserWindow MapLandBrowserWindow { get; private set; }

        public MapTileDetailWindow MapTileDetailWindow { get; private set; }

        public MapTilePreviewWindow MapTilePreviewWindow { get; private set; }

        public MapConnectToServerWindow MapConnectToServerWindow { get; private set; }

        public override void Draw()
        {
            base.Draw();
            MapToolbarWindow.Draw();
            MapViewWindow.Draw();
            MapItemBrowserWindow.Draw();
            MapLandBrowserWindow.Draw();
            MapTileDetailWindow.Draw();
            MapTilePreviewWindow.Draw();
            MapConnectToServerWindow.Draw();
        }

        public override void LoadContent(
            GraphicsDevice graphicsDevice,
            ContentManager contentManager,
            ImGuiRenderer guiRenderer)
        {
            base.LoadContent(graphicsDevice, contentManager, guiRenderer);

            const int size = 32;

            _mapControlLogin = contentManager.Load<Texture2D>($"Content/Controls/control-login-{size}");
            _mapControlLogout = contentManager.Load<Texture2D>($"Content/Controls/control-logout-{size}");

            _mapToolTerrainElevate = contentManager.Load<Texture2D>($"Content/Tools/terrain-elevate-{size}");
            _mapToolTerrainLower = contentManager.Load<Texture2D>($"Content/Tools/terrain-lower-{size}");
            _mapToolTerrainAdd = contentManager.Load<Texture2D>($"Content/Tools/terrain-add-{size}");
            _mapToolTerrainSubtract = contentManager.Load<Texture2D>($"Content/Tools/terrain-subtract-{size}");
            _mapToolTerrainCoast = contentManager.Load<Texture2D>($"Content/Tools/terrain-coast-{size}");
            _mapToolTerrainFlatten = contentManager.Load<Texture2D>($"Content/Tools/terrain-flatten-{size}");
            _mapToolTerrainPaint = contentManager.Load<Texture2D>($"Content/Tools/terrain-paint-{size}");
            _mapToolTerrainSmooth = contentManager.Load<Texture2D>($"Content/Tools/terrain-smooth-{size}");

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

            var mapToolDescriptions = new[]
            {
                new ToolDescription { Group = ToolGroup.Control, Name = "Login", TextureHandle = mapControlLogin, Size = size },
                new ToolDescription { Group = ToolGroup.Control, Name = "Logout", TextureHandle = mapControlLogout, Size = size },
                new ToolDescription { Group = ToolGroup.Selection, Name = "Elevate", TextureHandle = mapToolTerrainElevate, Size = size },
                new ToolDescription { Group = ToolGroup.Selection, Name = "Lower", TextureHandle = mapToolTerrainLower, Size = size },
                new ToolDescription { Group = ToolGroup.Selection, Name = "Add", TextureHandle = mapToolTerrainAdd, Size = size },
                new ToolDescription { Group = ToolGroup.Selection, Name = "Subtract", TextureHandle = mapToolTerrainSubtract, Size = size },
                new ToolDescription { Group = ToolGroup.Selection, Name = "Coast", TextureHandle = mapToolTerrainCoast, Size = size },
                new ToolDescription { Group = ToolGroup.Selection, Name = "Flatten", TextureHandle = mapToolTerrainFlatten, Size = size },
                new ToolDescription { Group = ToolGroup.Selection, Name = "Paint", TextureHandle = mapToolTerrainPaint, Size = size },
                new ToolDescription { Group = ToolGroup.Selection, Name = "Smooth", TextureHandle = mapToolTerrainSmooth, Size = size },
            };

            MapToolbarWindow = new MapToolbarWindow(mapToolDescriptions);

            var renderTargetId = guiRenderer.BindTexture(_mapEditRenderTarget);

            MapViewWindow = new MapViewWindow(_mapEditState, renderTargetId);

            MapTileDetailWindow = new MapTileDetailWindow(_itemProvider);
            MapTileDetailWindow.LoadContent(graphicsDevice, contentManager, guiRenderer);
            MapTilePreviewWindow = new MapTilePreviewWindow();
            MapTilePreviewWindow.LoadContent(graphicsDevice, contentManager, guiRenderer);
            MapLandBrowserWindow = new MapLandBrowserWindow(_itemProvider, _tileDataProvider, MapTileDetailWindow, MapTilePreviewWindow);
            MapLandBrowserWindow.LoadContent(graphicsDevice, contentManager, guiRenderer);
            MapItemBrowserWindow = new MapItemBrowserWindow(_itemProvider, _tileDataProvider, MapTileDetailWindow, MapTilePreviewWindow);
            MapItemBrowserWindow.LoadContent(graphicsDevice, contentManager, guiRenderer);
            MapConnectToServerWindow = new MapConnectToServerWindow(_profileService);
            MapConnectToServerWindow.LoadContent(graphicsDevice, contentManager, guiRenderer);
        }

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

            base.UnloadContent();
        }
    }
}
