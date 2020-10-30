using System.IO;
using System.Text;

namespace UOStudio.Core.Ultima
{
    public class TileDataProvider : MulProvider
    {
        private int _staticCount;
        private readonly LandTileData[] _landTiles;
        private StaticTileData[] _staticTiles;

        public TileDataProvider(Stream stream, bool readOnly = false)
            : base(stream, readOnly)
        {
            _landTiles = new LandTileData[0x4000];
            Initialize();
        }

        public TileDataProvider(string fileName, bool readOnly = false)
            : base(fileName, readOnly)
        {
            _landTiles = new LandTileData[0x4000];
            Initialize();
        }

        public LandTileData[] LandTiles => _landTiles;

        public StaticTileData[] StaticTiles => _staticTiles;

        public TileData this[int id] => id < 0x4000
            ? (TileData)_landTiles[id]
            : _staticTiles[id - 0x4000];

        public override MulBlock GetBlock(int id) => GetData(id, 0);

        protected override MulBlock GetData(int id, int offset)
        {
            var block = id < 0x4000
                ? _landTiles[id].Clone()
                : _staticTiles[id].Clone();

            block.Id = id;
            block.Changed += OnChanged;
            block.Finished += OnFinished;
            return block;
        }

        protected override void SetData(int id, int offset, MulBlock block)
        {
            if (id >= 0x4000 + _staticCount)
            {
                return;
            }

            if (id < 0x4000)
            {
                _landTiles[id] = (LandTileData)block.Clone();
            }
            else
            {
                _staticTiles[id - 0x4000] = (StaticTileData)block.Clone();
            }

            if (!ReadOnly)
            {
                _dataStream.Position = offset;
                block.Write(_writer);
            }
        }

        protected override int CalculateOffset(int id) => GetTileDataOffset(id);

        private void Initialize()
        {
            var version = _dataStream.Length > 3188736
                ? TileDataVersion.HighSeas
                : TileDataVersion.Legacy;

            _dataStream.Position = 0;
            var reader = new BinaryReader(_dataStream, Encoding.Default, true);
            for (var i = 0; i < 0x4000; ++i)
            {
                if (version == TileDataVersion.Legacy && i % 32 == 0 ||
                    version >= TileDataVersion.HighSeas && (i == 1 || i > 1 && i % 32 == 0))
                {
                    _dataStream.Seek(4, SeekOrigin.Current);
                }
                _landTiles[i] = new LandTileData(reader, version);
            }

            _staticCount = (int)(_dataStream.Length - _dataStream.Position) / StaticTileData.StaticTileGroupSize * 32;
            _staticTiles = new StaticTileData[_staticCount];
            for (var i = 0; i < _staticCount; ++i)
            {
                if (i % 32 == 0)
                {
                    _dataStream.Seek(4, SeekOrigin.Current);
                }
                _staticTiles[i] = new StaticTileData(reader, version);
            }
        }

        private static int GetTileDataOffset(int id)
        {
            int group;
            int tile;
            if (id > 0x3fff)
            {
                id -= 0x4000;
                group = id / 32;
                tile = id % 32;

                return 512 * LandTileData.LandTileGroupSize +
                       group * StaticTileData.StaticTileGroupSize +
                       4 + tile * StaticTileData.StaticTileDataSize;
            }

            group = id / 32;
            tile = id % 32;
            return group * LandTileData.LandTileGroupSize +
                   4 + tile * LandTileData.LandTileDataSize;
        }

    }
}
