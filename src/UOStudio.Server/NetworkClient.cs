using System;
using LiteNetLib;
using LiteNetLib.Utils;

namespace UOStudio.Server
{
    internal class NetworkClient : IEquatable<NetworkClient>
    {
        private readonly NetPeer _peer;

        public NetworkClient(NetPeer peer)
        {
            _peer = peer;
            Id = _peer.Id;
            Address = _peer.EndPoint.Address.ToString();
            Port = _peer.EndPoint.Port;
        }

        public int Id { get; }

        public int WorldId { get; set; }

        public string Address { get; }

        public int Port { get; }

        public void Send(NetDataWriter writer, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            _peer.Send(writer, deliveryMethod);
        }

        public bool Equals(NetworkClient other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(_peer, other._peer) && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((NetworkClient)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_peer, Id);
        }
    }
}
