using System;

namespace UOStudio.Server
{
    public interface INetworkServer : IDisposable
    {
        void Start(int port);

        void Stop();
    }
}
