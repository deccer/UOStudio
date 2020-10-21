using System.IO;
using Newtonsoft.Json;
using Serilog;

namespace UOStudio.Core.Settings
{
    public sealed class ConfigurationSaver : IConfigurationSaver
    {
        private readonly ILogger _logger;

        public ConfigurationSaver(ILogger logger)
        {
            _logger = logger;
        }

        public void SaveConfiguration<T>(string fileName, T configuration) where T : class
        {
            _logger.Information($"Configuration - Writing to {fileName}...");
            File.WriteAllText(fileName, JsonConvert.SerializeObject(configuration, Formatting.Indented));
            _logger.Information($"Configuration - Writing to {fileName}...Done");
        }
    }
}
