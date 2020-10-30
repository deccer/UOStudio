using System.IO;
using System.Text;

namespace UOStudio.Core.Ultima
{
    public class StaticTileData : TileData
    {
        public const int StaticTileDataSize = 37;
        public const int StaticTileGroupSize = 4 + 32 * StaticTileDataSize;

        public StaticTileData(BinaryReader binaryReader, TileDataVersion tileDataVersion = TileDataVersion.Legacy)
        {
            _tileDataVersion = tileDataVersion;
            if (binaryReader != null)
            {
                ReadFlags(binaryReader);
                Weight = binaryReader.ReadByte();
                Quality = binaryReader.ReadByte();
                Unknown1 = binaryReader.ReadByte();
                Unknown2 = binaryReader.ReadByte();
                Quantity = binaryReader.ReadByte();
                AnimationId = binaryReader.ReadUInt16();
                Unknown3 = binaryReader.ReadByte();
                Hue = binaryReader.ReadByte();
                Unknown4 = binaryReader.ReadUInt16();
                Height = binaryReader.ReadByte();
                var nameBuffer = binaryReader.ReadBytes(20);
                TileName = Encoding.Default
                    .GetString(nameBuffer)
                    .Trim();
            }            
        }
        
        public byte Weight { get; set; }
        
        public byte Quality { get; set; }
        
        public byte Unknown1 { get; set; }
        
        public byte Unknown2 { get; set; }
        
        public byte Quantity { get; set; }
        
        public ushort AnimationId { get; set; }
        
        public byte Unknown3 { get; set; }
        
        public byte Hue { get; set; }
        
        public ushort Unknown4 { get; set; }
        
        public byte Height { get; set; }
        
        public void PopulateClone(StaticTileData staticTileDataClone)
        {
            base.PopulateClone(staticTileDataClone);
            staticTileDataClone.Weight = Weight;
            staticTileDataClone.Quality = Quality;
            staticTileDataClone.Unknown1 = Unknown1;
            staticTileDataClone.Unknown2 = Unknown2;
            staticTileDataClone.Quantity = Quantity;
            staticTileDataClone.AnimationId = AnimationId;
            staticTileDataClone.Unknown3 = Unknown3;
            staticTileDataClone.Hue = Hue;
            staticTileDataClone.Unknown4 = Unknown4;
            staticTileDataClone.Height = Height;
        }

        public override int GetSize() => StaticTileDataSize;

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
            writer.Write(Weight);
            writer.Write(Quality);
            writer.Write(Unknown1);
            writer.Write(Unknown2);
            writer.Write(Quantity);
            writer.Write(AnimationId);
            writer.Write(Unknown3);
            writer.Write(Hue);
            writer.Write(Unknown4);
            writer.Write(Height);
            writer.Write(nameBuffer);
        }

        public override MulBlock Clone()
        {
            var clone = new StaticTileData(null);
            PopulateClone(clone);
            return clone;
        }
    }
}
