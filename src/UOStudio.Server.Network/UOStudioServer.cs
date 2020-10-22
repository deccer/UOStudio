using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using Serilog;
using UOStudio.Server.Core.Settings;

namespace UOStudio.Server.Network
{
    public class UOStudioServer
    {
        private readonly ILogger _logger;
        private readonly IAppSettingsProvider _appSettingsProvider;
        private readonly EventBasedNetListener _listener;
        private readonly NetManager _server;

        public UOStudioServer(ILogger logger, IAppSettingsProvider appSettingsProvider)
        {
            _logger = logger;
            _appSettingsProvider = appSettingsProvider;
            _listener = new EventBasedNetListener();
            _server = new NetManager(_listener);
        }

        private void Initialize()
        {
            _server.Start(_appSettingsProvider.AppSettings.Network.Port);

            _listener.ConnectionRequestEvent += ConnectionRequestEventHandler;
            _listener.PeerConnectedEvent += PeerConnectedEventHandler;
        }

        public void Run()
        {
            Initialize();
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
                request.AcceptIfKey("UOStudio");
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
