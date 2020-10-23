using System.IO;
using Newtonsoft.Json;
using Serilog;

namespace UOStudio.Core
{
    public sealed class Saver : ISaver
    {
        private readonly ILogger _logger;

        public Saver(ILogger logger)
        {
            _logger = logger;
        }

        public void Save<T>(string fileName, T configuration) where T : class
        {
            _logger.Information($"Configuration - Writing to {fileName}...");
            File.WriteAllText(fileName, JsonConvert.SerializeObject(configuration, Formatting.Indented));
            _logger.Information($"Configuration - Writing to {fileName}...Done");
        }
    }
}
