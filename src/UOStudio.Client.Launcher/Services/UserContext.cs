using System;
using UOStudio.Client.Launcher.Contracts;

namespace UOStudio.Client.Launcher.Services
{
    public class UserContext : IUserContext
    {
        public Uri AuthBaseUri { get; set; }

        public Uri ApiBaseUri { get; set; }

        public string ServerName { get; set; }

        public int ServerPort { get; set; }

        public UserCredentials UserCredentials { get; set; }
        
        public string ConnectionTicket { get; set; }
    }
}