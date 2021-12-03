namespace UOStudio.Server.Common
{
    public class ServerSettings
    {
        public const string ServerSection = "Server";

        public int MaximumConnectedPeersCount { get; set; }

        public int Port { get; set; }

        public string CertificatesDirectory { get; set; }

        public string DatabaseDirectory { get; set; }

        public string ProjectsDirectory { get; set; }

        public string TemplatesDirectory { get; set; }

        public string TemplatesGitRepository { get; set; }

        public string DownloadsDirectory { get; set; }
    }
}
