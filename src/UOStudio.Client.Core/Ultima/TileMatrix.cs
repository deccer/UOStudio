using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UOStudio.Common.Core.IO;

namespace UOStudio.Client.Engine.Ultima
{
    public class TileMatrix : IDisposable
    {
        private static readonly List<TileMatrix> _instances = new List<TileMatrix>();

        private readonly int _fileIndex;
        private readonly List<TileMatrix> _fileShare = new List<TileMatrix>();

        private readonly BinaryReader _staticsIndexReader;

        private readonly LandTile[] _invalidLandBlock;
        private readonly int[][] _landPatches;
        private readonly LandTile[][][] _landTiles;

        private readonly UOPIndex _mapIndex;

        private readonly FileStream _mapStream;

        private readonly Map _map;

        private readonly int[][] _staticPatches;
        private readonly StaticTile[][][][][] _staticTiles;

        private readonly TileList _tileList = new TileList();

        private TileList[][] _lists;
        private StaticTile[] _tileBuffer = new StaticTile[128];

        public TileMatrix(Map map, int fileIndex, int mapID, int width, int height)
        {
            _map = map;
            lock (_instances)
            {
                for (var i = 0; i < _instances.Count; ++i)
                {
                    var tileMatrix = _instances[i];
                    if (tileMatrix._fileIndex == fileIndex)
                    {
                        lock (_fileShare)
                        {
                            lock (tileMatrix._fileShare)
                            {
                                tileMatrix._fileShare.Add(this);
                                _fileShare.Add(tileMatrix);
                            }
                        }
                    }
                }

                _instances.Add(this);
            }

            MapDirectory = _map.Directory;
            _fileIndex = fileIndex;
            BlockWidth = width >> 3;
            BlockHeight = height >> 3;

            _map = map;

            if (fileIndex != 0x7F)
            {
                var mapPath = Path.Combine(_map.Directory, $"map{fileIndex}LegacyMUL.uop");
                if (File.Exists(mapPath))
                {
                    _mapStream = new FileStream(mapPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    _mapIndex = new UOPIndex(_mapStream);
                }
                else
                {
                    mapPath = Path.Combine(_map.Directory, $"map{fileIndex}.mul");
                    if (File.Exists(mapPath))
                    {
                        _mapStream = new FileStream(mapPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    }
                }

                var staticsIndexPath = Path.Combine(_map.Directory, $"staidx{fileIndex}.mul");
                if (File.Exists(staticsIndexPath))
                {
                    StaticsIndexStream = new FileStream(staticsIndexPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    _staticsIndexReader = new BinaryReader(StaticsIndexStream);
                }

                var staticsPath = Path.Combine(_map.Directory, $"statics{fileIndex}.mul");
                if (File.Exists(staticsPath))
                {
                    StaticsDataStream = new FileStream(staticsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                }
            }

            EmptyStaticBlock = new StaticTile[8][][];

            for (var i = 0; i < 8; ++i)
            {
                EmptyStaticBlock[i] = new StaticTile[8][];

                for (var j = 0; j < 8; ++j)
                {
                    EmptyStaticBlock[i][j] = Array.Empty<StaticTile>();
                }
            }

            _invalidLandBlock = new LandTile[196];

            _landTiles = new LandTile[BlockWidth][][];
            _staticTiles = new StaticTile[BlockWidth][][][][];
            _staticPatches = new int[BlockWidth][];
            _landPatches = new int[BlockWidth][];

            Patch = new TileMatrixPatch(this, mapID);
        }

        public string MapDirectory { get; }

        public FileStream StaticsIndexStream { get; }

        public FileStream StaticsDataStream { get; }

        public TileMatrixPatch Patch { get; }

        public int BlockWidth { get; }

        public int BlockHeight { get; }

        public StaticTile[][][] EmptyStaticBlock { get; }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetStaticBlock(int x, int y, StaticTile[][][] value)
        {
            if (x < 0 || y < 0 || x >= BlockWidth || y >= BlockHeight)
            {
                return;
            }

            _staticTiles[x] ??= new StaticTile[BlockHeight][][][];
            _staticTiles[x][y] = value;

            _staticPatches[x] ??= new int[(BlockHeight + 31) >> 5];
            _staticPatches[x][y >> 5] |= 1 << (y & 0x1F);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public StaticTile[][][] GetStaticBlock(int x, int y)
        {
            if (x < 0 || y < 0 || x >= BlockWidth || y >= BlockHeight || StaticsDataStream == null || StaticsIndexStream == null)
            {
                return EmptyStaticBlock;
            }

            _staticTiles[x] ??= new StaticTile[BlockHeight][][][];

            var tiles = _staticTiles[x][y];

            if (tiles != null)
            {
                return tiles;
            }

            lock (_fileShare)
            {
                for (var i = 0; tiles == null && i < _fileShare.Count; ++i)
                {
                    var shared = _fileShare[i];

                    lock (shared)
                    {
                        if (x < shared.BlockWidth && y < shared.BlockHeight)
                        {
                            var theirTiles = shared._staticTiles[x];

                            if (theirTiles != null)
                            {
                                tiles = theirTiles[y];
                            }

                            if (tiles != null)
                            {
                                var theirBits = shared._staticPatches[x];

                                if (theirBits != null && (theirBits[y >> 5] & (1 << (y & 0x1F))) != 0)
                                {
                                    tiles = null;
                                }
                            }
                        }
                    }
                }
            }

            return _staticTiles[x][y] = tiles ?? ReadStaticBlock(x, y);
        }

        public StaticTile[] GetStaticTiles(int x, int y) => GetStaticBlock(x >> 3, y >> 3)[x & 0x7][y & 0x7];

        [MethodImpl(MethodImplOptions.Synchronized)]
        public StaticTile[] GetStaticTiles(int x, int y, bool multis)
        {
            var tiles = GetStaticBlock(x >> 3, y >> 3);
            return tiles[x & 0x7][y & 0x7];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetLandBlock(int x, int y, LandTile[] value)
        {
            if (x < 0 || y < 0 || x >= BlockWidth || y >= BlockHeight)
            {
                return;
            }

            _landTiles[x] ??= new LandTile[BlockHeight][];
            _landTiles[x][y] = value;

            _landPatches[x] ??= new int[(BlockHeight + 31) >> 5];
            _landPatches[x][y >> 5] |= 1 << (y & 0x1F);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public LandTile[] GetLandBlock(int x, int y)
        {
            if (x < 0 || y < 0 || x >= BlockWidth || y >= BlockHeight || _mapStream == null)
            {
                return _invalidLandBlock;
            }

            _landTiles[x] ??= new LandTile[BlockHeight][];

            var tiles = _landTiles[x][y];

            if (tiles != null)
            {
                return tiles;
            }
            return _landTiles[x][y] = tiles ?? ReadLandBlock(x, y);
        }

        public LandTile GetLandTile(int x, int y) => GetLandBlock(x >> 3, y >> 3)[((y & 0x7) << 3) + (x & 0x7)];

        [MethodImpl(MethodImplOptions.Synchronized)]
        private unsafe StaticTile[][][] ReadStaticBlock(int x, int y)
        {
            try
            {
                _staticsIndexReader.BaseStream.Seek((x * BlockHeight + y) * 12, SeekOrigin.Begin);

                var lookup = _staticsIndexReader.ReadInt32();
                var length = _staticsIndexReader.ReadInt32();

                if (lookup < 0 || length <= 0)
                {
                    return EmptyStaticBlock;
                }

                var count = length / 7;

                StaticsDataStream.Seek(lookup, SeekOrigin.Begin);

                if (_tileBuffer.Length < count)
                {
                    _tileBuffer = new StaticTile[count];
                }

                var staTiles = _tileBuffer;

                fixed (StaticTile* pTiles = staTiles)
                {
                    NativeReader.Read(StaticsDataStream.SafeFileHandle!.DangerousGetHandle(), pTiles, length);
                    if (_lists == null)
                    {
                        _lists = new TileList[8][];

                        for (var i = 0; i < 8; ++i)
                        {
                            _lists[i] = new TileList[8];

                            for (var j = 0; j < 8; ++j)
                            {
                                _lists[i][j] = new TileList();
                            }
                        }
                    }

                    var lists = _lists;

                    StaticTile* pCur = pTiles, pEnd = pTiles + count;

                    while (pCur < pEnd)
                    {
                        lists[pCur->_x & 0x7][pCur->_y & 0x7].Add(pCur->_id, pCur->_z);
                        pCur += 1;
                    }

                    var tiles = new StaticTile[8][][];

                    for (var i = 0; i < 8; ++i)
                    {
                        tiles[i] = new StaticTile[8][];

                        for (var j = 0; j < 8; ++j)
                        {
                            tiles[i][j] = lists[i][j].ToArray();
                        }
                    }

                    return tiles;
                }
            }
            catch (EndOfStreamException)
            {
                return EmptyStaticBlock;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private unsafe LandTile[] ReadLandBlock(int x, int y)
        {
            try
            {
                var offset = (x * BlockHeight + y) * 196 + 4;

                if (_mapIndex != null)
                {
                    offset = _mapIndex.Lookup(offset);
                }

                _mapStream.Seek(offset, SeekOrigin.Begin);

                var tiles = new LandTile[64];

                fixed (LandTile* pTiles = tiles)
                {
                    NativeReader.Read(_mapStream.SafeFileHandle!.DangerousGetHandle(), pTiles, 192);
                }

                return tiles;
            }
            catch
            {
                return _invalidLandBlock;
            }
        }

        public void Dispose()
        {
            _mapIndex?.Close();
            _mapStream?.Close();
            StaticsDataStream?.Close();
            _staticsIndexReader?.Close();
        }
    }
}
