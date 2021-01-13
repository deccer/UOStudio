using System.IO;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace UOStudio
{
    internal sealed class ProfileLoader : IProfileLoader
    {
        private readonly ILogger _logger;
        private readonly string _profilesDirectory;

        public ProfileLoader(ILogger logger, IConfiguration configuration)
        {
            _logger = logger.ForContext<ProfileLoader>();
            _profilesDirectory = configuration["ProfilesDirectory"];
        }

        public Result<Profile[]> LoadProfiles()
        {
            var profilesFileName = Path.Combine(_profilesDirectory, "profiles.json");
            if (File.Exists(profilesFileName))
            {
                _logger.Information($"Loading profiles '{profilesFileName}'");
                var json = File.ReadAllText(profilesFileName);
                var profiles = JsonSerializer.Deserialize<Profile[]>(json);
                return Result.Success(profiles);
            }

            return Result.Failure<Profile[]>($"Profiles store '{profilesFileName}' not found.");
        }
    }
}
