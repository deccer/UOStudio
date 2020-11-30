using LiteNetLib;
using LiteNetLib.Utils;
using UOStudio.Client.Core;
using UOStudio.Core.Network;

namespace UOStudio.Client.Network.Packets
{
    public readonly struct ClientConnectPacket
    {
        private readonly NetDataWriter _dataWriter;
        private readonly Profile _profile;

        public ClientConnectPacket(NetDataWriter dataWriter, Profile profile)
        {
            _dataWriter = dataWriter;
            _profile = profile;
        }

        public void Send(NetPeer peer)
        {
            _dataWriter.Reset();
            _dataWriter.Put(PacketIds.C2S.Connect);
            _dataWriter.Put(_profile.AccountName);
            _dataWriter.Put(_profile.AccountPassword);
            peer.Send(_dataWriter, DeliveryMethod.ReliableOrdered);
        }
    }
}
