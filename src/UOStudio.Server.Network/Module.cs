using Microsoft.Extensions.DependencyInjection;
using UOStudio.Server.Network.PacketHandlers;

namespace UOStudio.Server.Network
{
    public static class Module
    {
        public static void AddNetworkServer(this IServiceCollection services)
        {
            services.AddSingleton<IPacketHandler<ClientConnectPacket, int>, ClientConnectPacketHandler>();
            services.AddSingleton<IPacketProcessor, PacketProcessor>();
            services.AddSingleton<IAccountStore, AccountStore>();
            services.AddSingleton<INetworkServer, NetworkServer>();
        }
    }
}
