using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace UOStudio
{
    internal sealed class ProfileSaver : IProfileSaver
    {
        private readonly ILogger _logger;
        private readonly string _profilesDirectory;

        public ProfileSaver(
            ILogger logger,
            IConfiguration configuration)
        {
            _logger = logger.ForContext<ProfileSaver>();
            _profilesDirectory = configuration["ProfilesDirectory"];
        }

        public void SaveProfiles(IReadOnlyCollection<Profile> profiles)
        {
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(profiles, jsonOptions);

            if (!Directory.Exists(_profilesDirectory))
            {
                Directory.CreateDirectory(_profilesDirectory);
            }

            var profilesFileName = Path.Combine(_profilesDirectory, "profiles.json");

            File.WriteAllText(profilesFileName, json);

            _logger.Information($"Profiles saved to '{profilesFileName}'.");
        }
    }
}
