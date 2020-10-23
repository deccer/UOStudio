namespace UOStudio.Client.Core.Settings
{
    public sealed class GeneralSettings
    {
        public GeneralSettings()
        {
            UltimaOnlineBasePath = string.Empty;
        }

        public string UltimaOnlineBasePath { get; set; }
    }
}
