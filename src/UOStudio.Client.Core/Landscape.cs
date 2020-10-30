using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using UOStudio.Core.Ultima;

namespace UOStudio.Client.Core
{
    public class Block
    {
        public Block(MapBlock mapBlock, StaticBlock staticBlock)
        {
            Map = mapBlock;
            Static = staticBlock;
        }

        public MapBlock Map { get; }

        public StaticBlock Static { get; }

        public void UpdateBlockAccess(Landscape landscape)
        {
            for (var i = 0; i < 64; ++i)
            {
                Map.Cells[i].CanBeEdited = landscape.CanWrite(Map.Cells[i], Map.Cells[i].Y);
            }
        }
    }

    public delegate void LandscapeChangeEvent();

    public delegate void StaticChangedEvent(StaticItem staticItem);

    public delegate void NewBlockEvent(Block block);

    public delegate void MapChangedEvent(MapCell mapCell);

    public class Landscape
    {
        protected MemoryCache _blockCache;

        public ushort Width { get; }
        public ushort Height { get; }
        public ushort CellWidth { get; }
        public ushort CellHeight { get; }

        public event LandscapeChangeEvent Changed;
        public event StaticChangedEvent StaticInserted;
        public event StaticChangedEvent StaticDeleted;
        public event StaticChangedEvent StaticElevated;
        public event StaticChangedEvent StaticHued;

        public SeparatedStaticBlock GetStaticBlock(int x, int y)
        {
            throw new NotImplementedException();
        }

        public MapBlock GetMapBlock(int x, int y)
        {
            throw new NotImplementedException();
        }

        public MapCell GetMapCell(int x, int y)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<StaticItem> GetStaticItems(int x, int y)
        {
            throw new NotImplementedException();
        }

        public Landscape(ushort width, ushort height)
        {
            _blockCache = new MemoryCache(new MemoryCacheOptions());
        }

        public sbyte GetEffectiveAltitude(MapCell tile)
        {
            var north = tile.Altitude;
            var west = GetLandAltitude(tile.X, tile.Y + 1, north);
            var south = GetLandAltitude(tile.X + 1, tile.Y + 1, north);
            var east = GetLandAltitude(tile.X + 1, tile.Y, north);
            return north;
        }

        public sbyte GetLandAltitude(int x, int y, sbyte defaultValue)
        {
            var cell = GetMapCell(x, y);
            return cell?.Altitude ?? defaultValue;
        }

        public bool CanWrite(MapCell mapCell, in ushort @ushort)
        {
            throw new NotImplementedException();
        }
    }
}
