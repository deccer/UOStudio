using LiteNetLib.Utils;

namespace UOStudio.Server.Network.PacketHandlers
{
    public class ClientConnectPacket : IPacket
    {
        public ClientConnectPacket(NetDataReader reader)
        {
            UserName = reader.GetString();
            PasswordHash = reader.GetString();
        }

        public string UserName { get; }

        public string PasswordHash { get; }
    }
}
