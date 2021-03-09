using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace UOStudio.Client
{
    public sealed class ProfileService : IProfileService
    {
        private readonly ILogger _logger;
        private readonly IList<Profile> _profiles;

        public ProfileService(ILogger logger)
        {
            _logger = logger.ForContext<ProfileService>();
            _profiles = new List<Profile>
            {
                new Profile
                {
                    Name = "UO:Renaissance",
                    ServerName = "13thrones.online",
                    ServerPort = 1234,
                    UserName = "rex",
                    Password = @"p455\/\/0rd"
                },
                new Profile
                {
                    Name = "UO:Outlands",
                    ServerName = "outlands.ultima.online",
                    ServerPort = 4321,
                    UserName = "owyn",
                    Password = "jaedan"
                },
                new Profile
                {
                    Name = "localhost:admin",
                    ServerName = "localhost",
                    ServerPort = 9050,
                    UserName = "admin",
                    Password = "admin"
                },
                new Profile
                {
                    Name = "localhost:notallowed",
                    ServerName = "localhost",
                    ServerPort = 9050,
                    UserName = "notallowed",
                    Password = "notallowed"
                }
            };
        }

        public Profile GetProfile(string profileName)
            => _profiles.FirstOrDefault(p => p.Name.Equals(profileName));

        public string[] GetProfileNames()
            => _profiles.OrderBy(x => x.Name).Select(p => p.Name).ToArray();

        public void DeleteProfile(Profile profile)
        {
            _profiles.Remove(profile);
        }

        public void CreateProfile(
            string name,
            string serverName,
            int serverPort,
            string userName,
            string password)
        {
            var profile = new Profile
            {
                Name = name,
                ServerName = serverName,
                ServerPort = serverPort,
                UserName = userName,
                Password = password
            };
            _profiles.Add(profile);
        }
    }
}
