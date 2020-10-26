using System;

namespace UOStudio.Client.Engine.Windows
{
    public sealed class ConnectEventArgs : EventArgs
    {
        public ConnectEventArgs(string serverName, int serverPort, string userName, string password)
        {
            ServerName = serverName;
            ServerPort = serverPort;
            UserName = userName;
            Password = password;
        }

        public string ServerName { get; }

        public int ServerPort { get; }

        public string UserName { get; }

        public string Password { get; }
    }
}
