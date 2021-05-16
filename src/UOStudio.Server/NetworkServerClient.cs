using LiteNetLib;

namespace UOStudio.Server
{
    internal class NetworkServerClient
    {
        private readonly NetPeer _peer;

        public NetworkServerClient(NetPeer peer, int accountId)
        {
            _peer = peer;
            AccountId = accountId;
        }

        public int AccountId { get; }
    }
}
