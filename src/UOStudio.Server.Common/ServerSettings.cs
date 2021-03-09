namespace UOStudio.Server.Common
{
    public class ServerSettings
    {
        public int MaximumConnectedPeersCount { get; set; }

        public int Port { get; set; }

        public string DatabaseLocation { get; set; }

        public string TemplateDirectory { get; set; }

        public string TemplateGitRepository { get; set; }
    }
}
