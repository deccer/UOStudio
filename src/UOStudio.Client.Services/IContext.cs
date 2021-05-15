using System.Security.Principal;

namespace UOStudio.Client.Services
{
    public interface IContext
    {
        IPrincipal User { get; set; }
    }
}
