using System;
using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UOStudio.Client.Engine.UI;
using Num = System.Numerics;

namespace UOStudio.Client.Engine.Windows.MapEdit
{
    public class MapItemBrowserWindow : Window
    {
        private readonly IItemProvider _itemProvider;
        private readonly TileDataProvider _tileDataProvider;
        private readonly MapTileDetailWindow _mapTileDetailWindow;
        private readonly MapTilePreviewWindow _mapTilePreviewWindow;
        private readonly IDictionary<Texture2D, IntPtr> _itemTexturesMap;
        private readonly IDictionary<IntPtr, int> _itemIdMap;
        private readonly IDictionary<int, string> _itemNameMap;

        private string _itemNameFilter; // TODO(deccer): add config
        private int _itemsPerRow = 3; // TODO(deccer): add config

        public MapItemBrowserWindow(
            IItemProvider itemProvider,
            TileDataProvider tileDataProvider,
            MapTileDetailWindow mapTileDetailWindow,
            MapTilePreviewWindow mapTilePreviewWindow)
            : base("Items")
        {
            _itemProvider = itemProvider;
            _tileDataProvider = tileDataProvider;
            _mapTileDetailWindow = mapTileDetailWindow;
            _mapTilePreviewWindow = mapTilePreviewWindow;

            _itemTexturesMap = new Dictionary<Texture2D, IntPtr>();
            _itemIdMap = new Dictionary<IntPtr, int>();
            _itemNameMap = new Dictionary<int, string>();
            _itemNameFilter = string.Empty;
        }

        protected override void DrawInternal()
        {
            ImGui.TextUnformatted("Filter");
            ImGui.SameLine();
            ImGui.InputText("##itemName", ref _itemNameFilter, 20);
            ImGui.TextUnformatted("Items per row");
            ImGui.SameLine();
            ImGui.InputInt("##itemsPerRow", ref _itemsPerRow);

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Num.Vector2.Zero);
            var perRowCounter = 0;

            foreach (var itemTexture in _itemTexturesMap)
            {
                var itemId = _itemIdMap[itemTexture.Value];
                //var itemName = _itemNameMap[itemId];
                var itemName = _itemNameMap.TryGetValue(itemId, out var name) ? name : "NOF";
                if (!itemName.Contains(_itemNameFilter, StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(_itemNameFilter))
                {
                    continue;
                }

                if (perRowCounter == _itemsPerRow)
                {
                    perRowCounter = 0;
                }

                if (ImGui.ImageButton(itemTexture.Value, new Num.Vector2(itemTexture.Key.Width, itemTexture.Key.Height)))
                {
                    _mapTileDetailWindow.IsLandSelected = false;
                    _mapTilePreviewWindow.SelectedTextureId = itemTexture.Value;
                    _mapTilePreviewWindow.SelectedTextureWidth = itemTexture.Key.Width;
                    _mapTilePreviewWindow.SelectedTextureHeight = itemTexture.Key.Height;
                    _mapTileDetailWindow.SelectedItemData = _tileDataProvider.ItemTable[itemId];
                }

                if (perRowCounter > 0)
                {
                    ImGui.SameLine();
                }

                perRowCounter++;
            }
            ImGui.PopStyleVar();
        }

        protected override ImGuiWindowFlags GetWindowFlags() => base.GetWindowFlags() | ImGuiWindowFlags.HorizontalScrollbar;

        protected override void LoadContentInternal(
            GraphicsDevice graphicsDevice,
            ContentManager contentManager,
            ImGuiRenderer imGuiRenderer)
        {
            Clear(imGuiRenderer);
            var itemCount = _tileDataProvider.ItemTable?.Length;
            for (var itemIndex = 0; itemIndex < itemCount; ++itemIndex)
            {
                var itemTexture = _itemProvider.GetStatic(graphicsDevice, itemIndex);
                if (itemTexture == null)
                {
                    continue;
                }

                var itemData = _tileDataProvider.ItemTable[itemIndex];
                if (!string.IsNullOrEmpty(itemData.Name))
                {
                    _itemNameMap.Add(itemIndex, itemData.Name);
                }

                var textureHandle = imGuiRenderer.BindTexture(itemTexture);
                _itemTexturesMap.Add(itemTexture, textureHandle);
                _itemIdMap.Add(textureHandle, itemIndex);
            }
        }

        private void Clear(ImGuiRenderer imGuiRenderer)
        {
            _itemIdMap.Clear();
            _itemNameMap.Clear();
            foreach (var (texture, texturePtr) in _itemTexturesMap)
            {
                texture.Dispose();
                imGuiRenderer.UnbindTexture(texturePtr);
            }
        }
    }
}
