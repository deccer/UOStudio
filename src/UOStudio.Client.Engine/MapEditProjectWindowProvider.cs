using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UOStudio.Client.Core.Settings;
using UOStudio.Client.Engine.UI;
using UOStudio.Client.Engine.Windows.General;
using UOStudio.Client.Engine.Windows.MapEdit;

namespace UOStudio.Client.Engine.Windows
{
    public sealed class MapEditProjectWindowProvider : CommonProjectWindowProvider
    {
        private readonly IItemProvider _itemProvider;
        private readonly TileDataProvider _tileDataProvider;
        private readonly MapEditState _mapEditState;
        private readonly RenderTarget2D _mapEditRenderTarget;

        private Texture2D _mapToolTerrainRaise;
        private Texture2D _mapToolTerrainLower;
        private Texture2D _mapToolTerrainFloodfill;
        private Texture2D _mapToolTerrainAutoShore;

        public MapEditProjectWindowProvider(
            IAppSettingsProvider appSettingsProvider,
            IFileVersionProvider fileVersionProvider,
            IItemProvider itemProvider,
            TileDataProvider tileDataProvider,
            MapEditState mapEditState,
            RenderTarget2D mapEditRenderTarget)
            : base(appSettingsProvider, fileVersionProvider)
        {
            _itemProvider = itemProvider;
            _tileDataProvider = tileDataProvider;
            _mapEditState = mapEditState;
            _mapEditRenderTarget = mapEditRenderTarget;
            DockSpaceWindow = new DockSpaceWindow("MapEdit");
        }

        public DockSpaceWindow DockSpaceWindow { get; }

        public MapToolbarWindow MapToolbarWindow { get; private set; }

        public MapViewWindow MapViewWindow { get; private set; }

        public MapItemBrowserWindow MapItemBrowserWindow { get; private set; }

        public MapLandBrowserWindow MapLandBrowserWindow { get; private set; }

        public MapTileDetailWindow MapTileDetailWindow { get; private set; }

        public MapTilePreviewWindow MapTilePreviewWindow { get; private set; }

        public override void Draw()
        {
            DockSpaceWindow.Draw();
            base.Draw();
            MapToolbarWindow.Draw();
            MapViewWindow.Draw();
            MapItemBrowserWindow.Draw();
            MapLandBrowserWindow.Draw();
            MapTileDetailWindow.Draw();
            MapTilePreviewWindow.Draw();
        }

        public override void LoadContent(
            GraphicsDevice graphicsDevice,
            ContentManager contentManager,
            ImGuiRenderer guiRenderer)
        {
            base.LoadContent(graphicsDevice, contentManager, guiRenderer);

            _mapToolTerrainLower = contentManager.Load<Texture2D>("Content/tools_terrain_lower_32");
            _mapToolTerrainRaise = contentManager.Load<Texture2D>("Content/tools_terrain_raise_32");
            _mapToolTerrainFloodfill = contentManager.Load<Texture2D>("Content/tools_terrain_floodfill_32");
            _mapToolTerrainAutoShore = contentManager.Load<Texture2D>("Content/tools_auto_shore_32");

            var mapToolTerrainLower = guiRenderer.BindTexture(_mapToolTerrainLower);
            var mapToolTerrainRaise = guiRenderer.BindTexture(_mapToolTerrainRaise);
            var mapToolTerrainFloodFill = guiRenderer.BindTexture(_mapToolTerrainFloodfill);
            var mapToolTerrainAutoShore = guiRenderer.BindTexture(_mapToolTerrainAutoShore);

            var mapToolDescriptions = new[]
            {
                new ToolDescription { Group = ToolGroup.Selection, Name = "Lower Terrain", TextureHandle = mapToolTerrainLower },
                new ToolDescription { Group = ToolGroup.Selection, Name = "Raise Terrain", TextureHandle = mapToolTerrainRaise },
                new ToolDescription { Group = ToolGroup.Selection, Name = "Flood Fill", TextureHandle = mapToolTerrainFloodFill },
                new ToolDescription { Group = ToolGroup.Selection, Name = "Auto Shore", TextureHandle = mapToolTerrainAutoShore },
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
        }
    }
}
