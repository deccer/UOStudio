using System;
using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework;
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

        private bool _stretchLandTextures;
        private string _landNameFilter;

        public bool StretchLandTextures
        {
            get => _stretchLandTextures;
            set => _stretchLandTextures = value;
        }

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
            Show();
        }

        protected override void DrawInternal()
        {
            var windowSize = ImGui.GetWindowSize();

            //ImGui.Checkbox("Stretch", ref _stretchLandTextures);
            ImGui.TextUnformatted("Filter");
            ImGui.SameLine();
            ImGui.InputText("##hidelabel", ref _landNameFilter, 20);

            ImGui.BeginGroup();
            var perRowIndex = 0;
            foreach (var landTexture in _landTexturesMap)
            {
                var landId = _landIdMap[landTexture.Value];
                var landName = _landNameMap[landId];
                if (!landName.Contains(_landNameFilter, StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(_landNameFilter))
                {
                    continue;
                }

                if (perRowIndex == (int)(windowSize.X / (_stretchLandTextures ? 88 : 44)) + 1)
                {
                    perRowIndex = 0;
                }

                if (_stretchLandTextures)
                {
                    if (ImGui.ImageButton(landTexture.Value, new Num.Vector2(88, 166)))
                    {
                        _mapTileDetailWindow.IsLandSelected = true;
                        _mapTilePreviewWindow.SelectedTextureId = landTexture.Value;
                        _mapTilePreviewWindow.SelectedTextureWidth = landTexture.Key.Width;
                        _mapTilePreviewWindow.SelectedTextureHeight = landTexture.Key.Height;
                        _mapTileDetailWindow.SelectedLandData = _tileDataProvider.LandTable[landId];
                    }
                }
                else
                {
                    if (ImGui.ImageButton(landTexture.Value, new Num.Vector2(landTexture.Key.Width, landTexture.Key.Height)))
                    {
                        _mapTileDetailWindow.IsLandSelected = true;
                        _mapTilePreviewWindow.SelectedTextureId = landTexture.Value;
                        _mapTilePreviewWindow.SelectedTextureWidth = landTexture.Key.Width;
                        _mapTilePreviewWindow.SelectedTextureHeight = landTexture.Key.Height;
                        _mapTileDetailWindow.SelectedLandData = _tileDataProvider.LandTable[landId];
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
            var landCount = _tileDataProvider.LandTable.Length;
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
    }
}
