using LiteNetLib.Utils;

namespace UOStudio.Server.Network.PacketHandlers
{
    public readonly struct ListProjectsPacket : IPacket
    {
        public ListProjectsPacket(NetDataReader reader) => ProjectId = reader.GetInt();

        public int ProjectId { get; }
    }
}
