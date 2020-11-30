using System;
using LiteNetLib;
using LiteNetLib.Utils;
using UOStudio.Core.Network;

namespace UOStudio.Client.Network.Packets
{
    public readonly struct CreateProjectPacket
    {
        private readonly NetDataWriter _dataWriter;
        private readonly Guid _accountId;
        private readonly string _projectName;

        public CreateProjectPacket(NetDataWriter dataWriter, Guid accountId, string projectName)
        {
            _dataWriter = dataWriter;
            _accountId = accountId;
            _projectName = projectName;
        }

        public void Send(NetPeer netPeer)
        {
            _dataWriter.Reset();
            _dataWriter.Put(PacketIds.C2S.CreateProject);
            _dataWriter.Put(_accountId.ToString("N"));
            _dataWriter.Put(_projectName);
            netPeer.Send(_dataWriter, DeliveryMethod.ReliableOrdered);
        }
    }
}
