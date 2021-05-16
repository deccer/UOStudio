using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteNetLib;
using UOStudio.Common.Contracts;

namespace UOStudio.Client.Services
{
    public interface INetworkClient
    {
        event Action<NetPeer> Connected;

        event Action Disconnected;

        Task ConnectAsync();

        void Disconnect();

        void Update();
    }
}
