using System.IO;
using Newtonsoft.Json;
using Serilog;
using UOStudio.Core;

namespace UOStudio.Client.Core
{
    public sealed class ProfileService : Repository<Profile>
    {
        private readonly ILogger _logger;
        private const string ProfilesFileName = "profiles.json";

        public ProfileService(ILogger logger)
        {
            _logger = logger.ForContext<ProfileService>();
            if (File.Exists(ProfilesFileName))
            {
                _logger.Debug("Profiles - Loading...");
                Load(ProfilesFileName);
                _logger.Debug("Profiles - Loading...Done.");
            }
            else
            {
                var localhostProfile = new Profile
                {
                    Name = "localhost",
                    Description = "Local Development",
                    ServerName = "localhost",
                    ServerPort = 9050,
                    AccountName = "Admin",
                    AccountPassword = "Password"
                };
                _logger.Debug("Profiles - Create default profile...");
                Add(localhostProfile);
                Save(ProfilesFileName);
                _logger.Debug("Profiles - Create default profile...Done.");
            }
        }

        public void Load(string fileName)
        {
            var json = File.ReadAllText(fileName);
            var items = JsonConvert.DeserializeObject<Profile[]>(json);
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Save(string fileName)
        {
            var json = JsonConvert.SerializeObject(_items, Formatting.Indented);
            File.WriteAllText(fileName, json);
        }
    }
}
