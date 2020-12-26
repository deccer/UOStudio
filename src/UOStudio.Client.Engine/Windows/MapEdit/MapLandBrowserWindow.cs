using System;
using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UOStudio.Client.Engine.UI;
using Num = System.Numerics;

namespace UOStudio.Client.Engine.Windows.MapEdit
{
    public class MapLandBrowserWindow : Window
    {
        private readonly IItemProvider _itemProvider;
        private readonly TileDataProvider _tileDataProvider;
        private readonly MapTileDetailWindow _mapTileDetailWindow;
        private readonly MapTilePreviewWindow _mapTilePreviewWindow;
        private readonly IDictionary<Texture2D, IntPtr> _landTexturesMap;
        private readonly IDictionary<IntPtr, int> _landIdMap;
        private readonly IDictionary<int, string> _landNameMap;

        private string _landNameFilter; // TODO(deccer): add config
        private int _itemsPerRow = 3; // TODO(deccer): add config

        public MapLandBrowserWindow(
            IItemProvider itemProvider,
            TileDataProvider tileDataProvider,
            MapTileDetailWindow mapTileDetailWindow,
            MapTilePreviewWindow mapTilePreviewWindow)
            : base("Lands")
        {
            _itemProvider = itemProvider;
            _tileDataProvider = tileDataProvider;
            _mapTileDetailWindow = mapTileDetailWindow;
            _mapTilePreviewWindow = mapTilePreviewWindow;
            _landTexturesMap = new Dictionary<Texture2D, IntPtr>();
            _landIdMap = new Dictionary<IntPtr, int>();
            _landNameMap = new Dictionary<int, string>();
            _landNameFilter = string.Empty;
        }

        protected override void DrawInternal()
        {
            ImGui.TextUnformatted("Filter");
            ImGui.SameLine();
            ImGui.InputText("##itemName", ref _landNameFilter, 20);
            ImGui.TextUnformatted("Items per row");
            ImGui.SameLine();
            ImGui.InputInt("##itemsPerRow", ref _itemsPerRow);

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Num.Vector2.Zero);
            var perRowCounter = 0;
            foreach (var landTexture in _landTexturesMap)
            {
                var landId = _landIdMap[landTexture.Value];
                //var landName = _landNameMap[landId];
                var landName = _landNameMap.TryGetValue(landId, out var name) ? name : "NOF";
                if (!landName.Contains(_landNameFilter, StringComparison.InvariantCultureIgnoreCase) &&
                    !string.IsNullOrEmpty(_landNameFilter))
                {
                    continue;
                }

                if (perRowCounter >= _itemsPerRow)
                {
                    perRowCounter = 0;
                }

                if (ImGui.ImageButton(landTexture.Value, new Num.Vector2(landTexture.Key.Width, landTexture.Key.Height)))
                {
                    _mapTileDetailWindow.IsLandSelected = true;
                    _mapTilePreviewWindow.SelectedTextureId = landTexture.Value;
                    _mapTilePreviewWindow.SelectedTextureWidth = landTexture.Key.Width;
                    _mapTilePreviewWindow.SelectedTextureHeight = landTexture.Key.Height;
                    _mapTileDetailWindow.SelectedLandData = _tileDataProvider.LandTable[landId];
                }

                if (perRowCounter > 0)
                {
                    ImGui.SameLine();
                }

                perRowCounter++;
            }
            ImGui.PopStyleVar();
        }

        protected override void LoadContentInternal(
            GraphicsDevice graphicsDevice,
            ContentManager contentManager,
            ImGuiRenderer imGuiRenderer)
        {
            Clear(imGuiRenderer);
            var landCount = _tileDataProvider.LandTable?.Length;
            for (var landIndex = 0; landIndex < landCount; ++landIndex)
            {
                var landTexture = _itemProvider.GetLand(graphicsDevice, landIndex);
                if (landTexture == null)
                {
                    continue;
                }

                var landData = _tileDataProvider.LandTable[landIndex];
                if (!string.IsNullOrEmpty(landData.Name))
                {
                    _landNameMap.Add(landIndex, landData.Name);
                }

                var textureHandle = imGuiRenderer.BindTexture((Texture2D)landTexture);
                _landTexturesMap.Add((Texture2D)landTexture, textureHandle);
                _landIdMap.Add(textureHandle, landIndex);
            }
        }

        private void Clear(ImGuiRenderer imGuiRenderer)
        {
            _landIdMap.Clear();
            _landNameMap.Clear();
            foreach (var (texture, texturePtr) in _landTexturesMap)
            {
                texture.Dispose();
                imGuiRenderer.UnbindTexture(texturePtr);
            }
        }
    }
}
