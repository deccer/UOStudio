using System.Runtime.InteropServices;

namespace UOStudio.Client.Engine.Ultima
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LandTile
    {
        internal short m_ID;
        internal sbyte m_Z;

        public int ID => m_ID;

        public int Z
        {
            get => m_Z;
            set => m_Z = (sbyte)value;
        }

        public int Height => 0;

        public bool Ignored => m_ID == 2 || m_ID == 0x1DB || m_ID >= 0x1AE && m_ID <= 0x1B5;

        public LandTile(short id, sbyte z)
        {
            m_ID = id;
            m_Z = z;
        }

        public void Set(short id, sbyte z)
        {
            m_ID = id;
            m_Z = z;
        }
    }
}
