using LiteNetLib;
using LiteNetLib.Utils;
using UOStudio.Core.Network;

namespace UOStudio.Client.Network.Packets
{
    public readonly struct CreateProjectPacket
    {
        private readonly NetDataWriter _dataWriter;

        public CreateProjectPacket(NetDataWriter dataWriter)
        {
            _dataWriter = dataWriter;
        }

        public void Send(NetPeer netPeer)
        {
            _dataWriter.Reset();
            _dataWriter.Put(PacketIds.CreateProject);
        }
    }
}
