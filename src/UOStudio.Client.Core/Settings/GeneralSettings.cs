namespace UOStudio.Client.Core.Settings
{
    public sealed class GeneralSettings
    {
        public GeneralSettings()
        {
            UltimaOnlineBasePath = string.Empty;
            UOStudioBaseUrl = "https://localhost:5001/";
            ProjectsPath = "Projects";
        }

        public string UltimaOnlineBasePath { get; set; }

        public string UOStudioBaseUrl { get; set; }

        public string ProjectsPath { get; set; }
    }
}
