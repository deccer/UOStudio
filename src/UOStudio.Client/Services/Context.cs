using System.Security.Principal;

namespace UOStudio.Client.Services
{
    public class Context : IContext
    {
        public IPrincipal User { get; set; }
    }
}
