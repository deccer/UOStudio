using Microsoft.Extensions.DependencyInjection;

namespace UOStudio.Web.Network
{
    public static class Module
    {
        public static void AddNetworkServer(this IServiceCollection services)
        {
            services.AddSingleton<INetworkServer, NetworkServer>();
        }
    }
}
