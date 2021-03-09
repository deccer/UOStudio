using System.IO;
using System.Runtime.CompilerServices;
using UOStudio.Common.Core.IO;

namespace UOStudio.Client.Engine.Ultima
{
    public class TileMatrixPatch
    {
        private readonly int _landBlocks;
        private readonly int _staticBlocks;

        private StaticTile[] _tileBuffer = new StaticTile[128];

        public TileMatrixPatch(TileMatrix matrix, int index)
        {
            if (!Enabled)
            {
                return;
            }

            var mapDataPath = Path.Combine(matrix.MapDirectory, $"mapdif{index}.mul");
            var mapIndexPath = Path.Combine(matrix.MapDirectory, $"mapdifl{index}.mul");

            if (File.Exists(mapDataPath) && File.Exists(mapIndexPath))
            {
                _landBlocks = PatchLand(matrix, mapDataPath, mapIndexPath);
            }

            var staDataPath = Path.Combine(matrix.MapDirectory, $"stadif{index}.mul");
            var staIndexPath = Path.Combine(matrix.MapDirectory, $"stadifl{index}.mul");
            var staLookupPath = Path.Combine(matrix.MapDirectory, $"stadifi{index}.mul");

            if (File.Exists(staDataPath) && File.Exists(staIndexPath) && File.Exists(staLookupPath))
            {
                _staticBlocks = PatchStatics(matrix, staDataPath, staIndexPath, staLookupPath);
            }
        }

        public static bool Enabled { get; set; } = true;

        public int LandBlocks
        {
            get
            {
                lock (this)
                {
                    return _landBlocks;
                }
            }
        }

        public int StaticBlocks
        {
            get
            {
                lock (this)
                {
                    return _staticBlocks;
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private unsafe int PatchLand(TileMatrix matrix, string dataPath, string indexPath)
        {
            using var patchDataStream = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var patchIndexStream = new FileStream(indexPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var patchIndexReader = new BinaryReader(patchIndexStream);

            var count = (int)(patchIndexReader.BaseStream.Length / 4);

            for (var i = 0; i < count; ++i)
            {
                var blockID = patchIndexReader.ReadInt32();
                var x = blockID / matrix.BlockHeight;
                var y = blockID % matrix.BlockHeight;

                patchDataStream.Seek(4, SeekOrigin.Current);

                var tiles = new LandTile[64];

                fixed (LandTile* pTiles = tiles)
                {
                    NativeReader.Read(patchDataStream.SafeFileHandle!.DangerousGetHandle(), pTiles, 192);
                }

                matrix.SetLandBlock(x, y, tiles);
            }

            patchIndexReader.Close();

            return count;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private unsafe int PatchStatics(TileMatrix matrix, string dataPath, string indexPath, string lookupPath)
        {
            using var staticsDataStream = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var staticsIndexStream = new FileStream(indexPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var staticsLookupStream = new FileStream(lookupPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var staticsIndexReader = new BinaryReader(staticsIndexStream);
            var staticsLookupReader = new BinaryReader(staticsLookupStream);

            var count = (int)(staticsIndexReader.BaseStream.Length / 4);

            var lists = new TileList[8][];

            for (var x = 0; x < 8; ++x)
            {
                lists[x] = new TileList[8];

                for (var y = 0; y < 8; ++y)
                {
                    lists[x][y] = new TileList();
                }
            }

            for (var i = 0; i < count; ++i)
            {
                var blockID = staticsIndexReader.ReadInt32();
                var blockX = blockID / matrix.BlockHeight;
                var blockY = blockID % matrix.BlockHeight;

                var offset = staticsLookupReader.ReadInt32();
                var length = staticsLookupReader.ReadInt32();
                staticsLookupReader.ReadInt32(); // Extra

                if (offset < 0 || length <= 0)
                {
                    matrix.SetStaticBlock(blockX, blockY, matrix.EmptyStaticBlock);
                    continue;
                }

                staticsDataStream.Seek(offset, SeekOrigin.Begin);

                var tileCount = length / 7;

                if (_tileBuffer.Length < tileCount)
                {
                    _tileBuffer = new StaticTile[tileCount];
                }

                var staTiles = _tileBuffer;

                fixed (StaticTile* pTiles = staTiles)
                {
                    NativeReader.Read(staticsDataStream.SafeFileHandle!.DangerousGetHandle(), pTiles, length);
                    StaticTile* pCur = pTiles, pEnd = pTiles + tileCount;

                    while (pCur < pEnd)
                    {
                        lists[pCur->X & 0x7][pCur->Y & 0x7].Add(pCur->Id, (sbyte)pCur->Z);
                        pCur += 1;
                    }

                    var tiles = new StaticTile[8][][];

                    for (var x = 0; x < 8; ++x)
                    {
                        tiles[x] = new StaticTile[8][];

                        for (var y = 0; y < 8; ++y)
                        {
                            tiles[x][y] = lists[x][y].ToArray();
                        }
                    }

                    matrix.SetStaticBlock(blockX, blockY, tiles);
                }
            }

            staticsIndexReader.Close();
            staticsLookupReader.Close();

            return count;
        }
    }
}
