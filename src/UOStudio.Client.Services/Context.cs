using System;
using System.Security.Principal;

namespace UOStudio.Client.Services
{
    public class Context : IContext
    {
        public IPrincipal User { get; set; }

        public Context()
        {
            User = new GenericPrincipal(new GenericIdentity(Environment.UserName), null);
        }
    }
}
