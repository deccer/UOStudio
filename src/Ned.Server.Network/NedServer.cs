using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using Serilog;

namespace Ned.Server.Network
{
    public class NedServer
    {
        private readonly ILogger _logger;
        private readonly EventBasedNetListener _listener;
        private readonly NetManager _server;

        public NedServer(ILogger logger)
        {
            _logger = logger;
            _listener = new EventBasedNetListener();
            _server = new NetManager(_listener);
        }

        private void Initialize(int port)
        {
            _server.Start(port);

            _listener.ConnectionRequestEvent += ConnectionRequestEventHandler;
            _listener.PeerConnectedEvent += PeerConnectedEventHandler;
        }

        public void Run(int port)
        {
            Initialize(port);
            while (_server.IsRunning)
            {
                _server.PollEvents();
                Thread.Sleep(15);
            }

            _server.Stop();
        }

        private void ConnectionRequestEventHandler(ConnectionRequest request)
        {
            if (_server.ConnectedPeersCount < 10)
            {
                request.AcceptIfKey("NCentrED");
            }
            else
            {
                request.Reject();
            }
        }

        private void PeerConnectedEventHandler(NetPeer peer)
        {
            _logger.Information("Server - Client Connected: {0}", peer.EndPoint);
            var writer = new NetDataWriter();
            writer.Put("Hello you majestic UO admirer!");
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }
    }
}
