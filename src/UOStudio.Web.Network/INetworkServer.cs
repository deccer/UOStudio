using System;

namespace UOStudio.Web.Network
{
    public interface INetworkServer : IDisposable
    {
        void Start(int port);

        void Stop();
    }
}
