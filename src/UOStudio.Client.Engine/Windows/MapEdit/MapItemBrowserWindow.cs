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

        private bool _stretchItemTextures;
        private string _itemNameFilter;

        public bool StretchItemTextures
        {
            get => _stretchItemTextures;
            set => _stretchItemTextures = value;
        }

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
            Show();
        }

        protected override void DrawInternal()
        {
            var windowSize = ImGui.GetWindowSize();

            ImGui.TextUnformatted("Filter");
            ImGui.SameLine();
            ImGui.InputText("##hidelabel", ref _itemNameFilter, 20);

            ImGui.BeginGroup();
            var perRowIndex = 0;
            foreach (var itemTexture in _itemTexturesMap)
            {
                var landId = _itemIdMap[itemTexture.Value];
                var landName = _itemNameMap[landId];
                if (!landName.Contains(_itemNameFilter, StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(_itemNameFilter))
                {
                    continue;
                }

                if (perRowIndex == (int)(windowSize.X / (_stretchItemTextures ? 88 : 44)) + 1)
                {
                    perRowIndex = 0;
                }

                if (_stretchItemTextures)
                {
                    if (ImGui.ImageButton(itemTexture.Value, new Num.Vector2(88, 166)))
                    {
                        _mapTileDetailWindow.IsLandSelected = false;
                        _mapTilePreviewWindow.SelectedTextureId = itemTexture.Value;
                        _mapTilePreviewWindow.SelectedTextureWidth = itemTexture.Key.Width;
                        _mapTilePreviewWindow.SelectedTextureHeight = itemTexture.Key.Height;
                        _mapTileDetailWindow.SelectedItemData = _tileDataProvider.ItemTable[landId];
                    }
                }
                else
                {
                    if (ImGui.ImageButton(itemTexture.Value, new Num.Vector2(itemTexture.Key.Width, itemTexture.Key.Height)))
                    {
                        _mapTileDetailWindow.IsLandSelected = false;
                        _mapTilePreviewWindow.SelectedTextureId = itemTexture.Value;
                        _mapTilePreviewWindow.SelectedTextureWidth = itemTexture.Key.Width;
                        _mapTilePreviewWindow.SelectedTextureHeight = itemTexture.Key.Height;
                        _mapTileDetailWindow.SelectedItemData = _tileDataProvider.ItemTable[landId];
                    }
                }

                if (perRowIndex > 0)
                {
                    ImGui.SameLine();
                }

                perRowIndex++;
            }
        }

        protected override void LoadContentInternal(
            GraphicsDevice graphicsDevice,
            ContentManager contentManager,
            ImGuiRenderer imGuiRenderer)
        {
            var itemCount = _tileDataProvider.ItemTable.Length;
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
    }
}
