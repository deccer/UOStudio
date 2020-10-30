using System.IO;

namespace UOStudio.Core.Ultima
{
    public abstract class TileData : MulBlock
    {
        protected TileDataVersion _tileDataVersion;
        protected uint _newFlags;

        public TileDataFlags Flags { get; set; }

        public string TileName { get; set; }

        protected void ReadFlags(BinaryReader binaryReader)
        {
            Flags = (TileDataFlags)binaryReader.ReadUInt32();
            if (_tileDataVersion == TileDataVersion.HighSeas)
            {
                _newFlags = binaryReader.ReadUInt32();
            }

        }

        protected void WriteFlags(BinaryWriter binaryWriter)
        {
            binaryWriter.Write((uint)Flags);
            if (_tileDataVersion == TileDataVersion.HighSeas)
            {
                binaryWriter.Write(_newFlags);
            }
        }

        protected void PopulateClone(TileData clone)
        {
            clone._tileDataVersion = _tileDataVersion;
            clone.Flags = Flags;
            clone._newFlags = _newFlags;
            clone.TileName = TileName;
        }
    }
}
