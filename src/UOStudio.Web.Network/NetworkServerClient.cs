using System;
using LiteNetLib;

namespace UOStudio.Web.Network
{
    internal class NetworkServerClient
    {
        private readonly NetPeer _peer;

        public NetworkServerClient(NetPeer peer, Guid accountId)
        {
            _peer = peer;
            AccountId = accountId.ToString("N");
        }

        public string AccountId { get; }
    }
}
