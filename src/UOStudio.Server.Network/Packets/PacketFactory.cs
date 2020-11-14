using System;
using System.Collections.Generic;
using LiteNetLib.Utils;

namespace UOStudio.Server.Network.Packets
{
    public class PacketFactory
    {
        private static readonly IDictionary<int, Type> _inputPacketTypes;
        private static readonly IDictionary<int, Type> _outputPacketTypes;

        static PacketFactory()
        {
            _inputPacketTypes = new Dictionary<int, Type>(64);
            _outputPacketTypes = new Dictionary<int, Type>(64);
        }
    }

    public abstract class Packet
    {
        protected Packet(NetDataReader reader)
        {
        }

        public NetDataWriter GetWriter()
        {
            var writer = new NetDataWriter();
            WriteToWriter(writer);
            return writer;
        }

        protected abstract void WriteToWriter(NetDataWriter writer);
    }
}
