using System.IO;

namespace UOStudio.Core.Ultima
{
    public abstract class MulBlock
    {
        public int Id { get; set; }

        public event MulBlockChangedEventHandler Changed;

        public event MulBlockChangedEventHandler Finished;

        public event MulBlockChangedEventHandler Destroyed;

        public abstract int GetSize();

        public abstract void Write(BinaryWriter writer);

        public abstract MulBlock Clone();

        public static void Change(MulBlock mulBlock)
        {
            mulBlock?.Changed?.Invoke(mulBlock);
        }

        public static void Finish(MulBlock mulBlock)
        {
            mulBlock?.Finished?.Invoke(mulBlock);
        }
    }
}
