using System;
using System.Collections.Generic;

namespace UOStudio.Client.Engine.Ultima
{
    public sealed class Map
    {
        private readonly int _fileIndex;

        private readonly object tileLock = new object();

        private TileMatrix _tiles;

        private static readonly IDictionary<string, (int FileIndex, int Width, int Height)> _mapDimensions;

        static Map() =>
            _mapDimensions = new Dictionary<string, (int FileIndex, int Width, int Height)>
            {
                { "Felucca", (0, 7168, 4096) },
                { "Trammel", (1, 7168, 4096) },
                { "Ilshenar", (2, 2304, 1600) },
                { "Malas", (3, 2560, 2048) },
                { "Tokuno", (4, 1448, 1448) },
                { "TerMur", (5, 1280, 4096) }
            };

        public Map(string directory, string name, int width = -1, int height = -1)
        {
            Directory = directory;
            Name = name;
            if (_mapDimensions.TryGetValue(name, out var mapSetup))
            {
                MapID = mapSetup.FileIndex;
                MapIndex = mapSetup.FileIndex;
                _fileIndex = mapSetup.FileIndex;
                Width = mapSetup.Width;
                Height = mapSetup.Height;
            }
            else
            {
                MapID = -1;
                MapIndex = -1;
                _fileIndex = -1;
                Width = width;
                Height = height;
            }
            TileData.Initialize(directory);
        }

        public string Name { get; }

        public TileMatrix Tiles
        {
            get
            {
                if (_tiles == null)
                {
                    lock (tileLock)
                    {
                        _tiles = new TileMatrix(this, _fileIndex, MapID, Width, Height);
                    }
                }

                return _tiles;
            }
        }

        public string Directory { get; }

        public int MapID { get; }

        public int MapIndex { get; }

        public int Width { get; }

        public int Height { get; }
    }
}
