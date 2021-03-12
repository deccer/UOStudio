namespace UOStudio.Server.Common
{
    public class ServerSettings
    {
        public int MaximumConnectedPeersCount { get; set; }

        public int Port { get; set; }

        public string DatabaseLocation { get; set; }

        public string ProjectsDirectory { get; set; }

        public string TemplatesDirectory { get; set; }

        public string TemplatesGitRepository { get; set; }
    }
}
