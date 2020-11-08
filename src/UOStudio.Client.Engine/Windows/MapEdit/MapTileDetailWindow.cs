using ImGuiNET;

namespace UOStudio.Client.Engine.Windows.MapEdit
{
    public class MapTileDetailWindow : Window
    {
        private readonly IItemProvider _itemProvider;

        public MapTileDetailWindow(IItemProvider itemProvider)
            : base("Details")
        {
            _itemProvider = itemProvider;
            Show();
        }

        private ItemData _selectedItemData;

        public ItemData SelectedItemData
        {
            get => _selectedItemData;
            set => _selectedItemData = value;
        }

        private LandData _selectedLandData;

        public LandData SelectedLandData
        {
            get => _selectedLandData;
            set => _selectedLandData = value;
        }

        public bool IsLandSelected { get; set; }

        protected override void DrawInternal()
        {
            ImGui.Columns(2);
            if (IsLandSelected)
            {
                if (!string.IsNullOrEmpty(_selectedLandData.Name))
                {
                    ImGui.TextUnformatted("name");
                    ImGui.NextColumn();
                    ImGui.TextUnformatted(_selectedLandData.Name);
                    ImGui.NextColumn();
                }

                foreach (var property in TileDataProvider.LandFlagsToPropertyList(_selectedLandData))
                {
                    ImGui.TextUnformatted(property);
                    ImGui.NextColumn();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(_selectedItemData.Name))
                {
                    ImGui.TextUnformatted("name");
                    ImGui.NextColumn();
                    ImGui.TextUnformatted(_selectedItemData.Name);
                    ImGui.NextColumn();
                }

                foreach (var property in TileDataProvider.ItemFlagsToPropertyList(_selectedItemData))
                {
                    ImGui.TextUnformatted(property);
                    ImGui.NextColumn();
                }
            }
        }
    }
}
