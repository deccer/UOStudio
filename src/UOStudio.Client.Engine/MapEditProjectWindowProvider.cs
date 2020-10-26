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
        private readonly MapEditState _mapEditState;
        private readonly RenderTarget2D _mapEditRenderTarget;

        private Texture2D _mapToolTerrainRaise;
        private Texture2D _mapToolTerrainLower;
        private Texture2D _mapToolTerrainFloodfill;
        private Texture2D _mapToolTerrainAutoShore;

        public MapEditProjectWindowProvider(
            IAppSettingsProvider appSettingsProvider,
            IFileVersionProvider fileVersionProvider,
            MapEditState mapEditState,
            RenderTarget2D mapEditRenderTarget)
            : base(appSettingsProvider, fileVersionProvider)
        {
            _mapEditState = mapEditState;
            _mapEditRenderTarget = mapEditRenderTarget;
            DockSpaceWindow = new DockSpaceWindow("MapEdit");

            MapItemBrowserWindow = new MapItemBrowserWindow();
            MapLandBrowserWindow = new MapLandBrowserWindow();
        }

        public DockSpaceWindow DockSpaceWindow { get; }

        public MapToolbarWindow MapToolbarWindow { get; private set; }

        public MapViewWindow MapViewWindow { get; private set; }

        public MapItemBrowserWindow MapItemBrowserWindow { get; }

        public MapLandBrowserWindow MapLandBrowserWindow { get; }

        public override void Draw()
        {
            DockSpaceWindow.Draw();
            base.Draw();
            MapToolbarWindow.Draw();
            MapViewWindow.Draw();
            MapItemBrowserWindow.Draw();
            MapLandBrowserWindow.Draw();
        }

        public override void LoadContent(ContentManager contentManager, ImGuiRenderer guiRenderer)
        {
            base.LoadContent(contentManager, guiRenderer);

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
        }
    }
}
