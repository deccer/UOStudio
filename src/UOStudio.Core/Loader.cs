using System;
using System.IO;
using Newtonsoft.Json;
using Serilog;

namespace UOStudio.Core
{
    public sealed class Loader : ILoader
    {
        private readonly ILogger _logger;

        public Loader(ILogger logger) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public T Load<T>(string fileName) where T : class
        {
            if (!File.Exists(fileName))
            {
                _logger.Warning($"Configuration - File: '{fileName}' does not exist.");
                return null;
            }

            _logger.Information($"Configuration - Reading from {fileName}...");
            var configurationContent = File.ReadAllText(fileName);
            _logger.Information($"Configuration - Reading from {fileName}...Done.");
            return JsonConvert.DeserializeObject<T>(configurationContent);
        }
    }
}
