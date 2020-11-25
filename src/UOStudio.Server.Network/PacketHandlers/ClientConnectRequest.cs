using LiteNetLib.Utils;

namespace UOStudio.Server.Network.PacketHandlers
{
    public readonly struct ClientConnectRequest : IPacket
    {
        public ClientConnectRequest(NetDataReader reader)
        {
            UserName = reader.GetString();
            PasswordHash = reader.GetString();
        }

        public string UserName { get; }

        public string PasswordHash { get; }
    }
}
