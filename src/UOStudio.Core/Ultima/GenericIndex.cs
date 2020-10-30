using System.IO;

namespace UOStudio.Core.Ultima
{
    public class GenericIndex : MulBlock
    {
        public int Lookup { get; set; }

        public int Size { get; set; }

        public int Various { get; set; }

        public GenericIndex(BinaryReader binaryReader)
        {
            if (binaryReader != null)
            {
                Lookup = binaryReader.ReadInt32();
                Size = binaryReader.ReadInt32();
                Various = binaryReader.ReadInt32();
            }
        }

        public override int GetSize() => 12;

        public override void Write(BinaryWriter writer)
        {
            if (writer != null)
            {
                writer.Write(Lookup);
                writer.Write(Size);
                writer.Write(Various);
            }
        }

        public override MulBlock Clone()
        {
            var index = new GenericIndex(null);
            index.Lookup = Lookup;
            index.Size = Size;
            index.Various = Various;
            return index;
        }
    }
}
