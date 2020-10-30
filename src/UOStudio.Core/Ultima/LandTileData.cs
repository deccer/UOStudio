using System.IO;
using System.Text;

namespace UOStudio.Core.Ultima
{
    public class LandTileData : TileData
    {
        public const int LandTileDataSize = 26;
        public const int LandTileGroupSize = 4 + 32 * LandTileDataSize;

        protected ushort _textureId;

        public LandTileData(BinaryReader binaryReader, TileDataVersion tileDataVersion = TileDataVersion.Legacy)
        {
            _tileDataVersion = tileDataVersion;
            if (binaryReader != null)
            {
                ReadFlags(binaryReader);
                _textureId = binaryReader.ReadUInt16();

                var nameBuffer = new byte[20];
                var count = 0;
                var byteRead = binaryReader.ReadByte();
                while (count < 20 && byteRead != 0)
                {
                    nameBuffer[count] = byteRead;
                    byteRead = binaryReader.ReadByte();
                    count++;
                }

                TileName = Encoding.Default
                    .GetString(nameBuffer)
                    .Trim();
            }
        }

        public override int GetSize() => LandTileDataSize;

        public override void Write(BinaryWriter writer)
        {
            var nameBuffer = new byte[20];
            for (var i = 0; i < nameBuffer.Length; ++i)
            {
                if (i >= TileName.Length)
                {
                    nameBuffer[i] = 0;
                }
                else
                {
                    nameBuffer[i] = (byte)TileName[i];
                }
            }

            WriteFlags(writer);
            writer.Write(_textureId);
            writer.Write(nameBuffer);
        }

        public override MulBlock Clone()
        {
            var landTileData = new LandTileData(null);
            PopulateClone(landTileData);
            return landTileData;
        }

        protected void PopulateClone(LandTileData landTileDataClone)
        {
            base.PopulateClone(landTileDataClone);
            landTileDataClone._textureId = _textureId;
        }
    }
}
