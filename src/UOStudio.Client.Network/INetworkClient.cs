using System;
using System.Net;
using UOStudio.Client.Core;

namespace UOStudio.Client.Network
{
    public interface INetworkClient
    {
        event Action<EndPoint, int> Connected;

        event Action Disconnected;

        bool IsConnected { get; }

        void Connect(Profile profile);

        void Disconnect();

        void SendMessage(Guid accountId, string message);
    }
}
