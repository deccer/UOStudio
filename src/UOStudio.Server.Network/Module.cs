using Microsoft.Extensions.DependencyInjection;
using UOStudio.Server.Network.PacketHandlers;

namespace UOStudio.Server.Network
{
    public static class Module
    {
        public static void AddNetworkServer(this IServiceCollection services)
        {
            services.AddSingleton<IRequestHandler<ClientConnectRequest, ClientConnectResult>, ClientConnectRequestHandler>();
            services.AddSingleton<IRequestHandler<CreateProjectRequest, CreateProjectResult>, CreateProjectRequestHandler>();
            services.AddSingleton<IRequestProcessor, RequestProcessor>();
            services.AddSingleton<INetworkServer, NetworkServer>();
        }
    }
}
