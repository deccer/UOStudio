using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using Serilog;

namespace UOStudio
{
    internal sealed class ProfileService : IProfileService
    {
        private readonly ILogger _logger;
        private readonly IProfileSaver _profileSaver;
        private readonly IProfileLoader _profileLoader;

        private IList<Profile> _profiles;

        public ProfileService(
            ILogger logger,
            IProfileSaver profileSaver,
            IProfileLoader profileLoader)
        {
            _logger = logger.ForContext<ProfileService>();
            _profileSaver = profileSaver;
            _profileLoader = profileLoader;
            LoadProfiles();
        }

        public Result AddProfile(Profile profile)
        {
            if (_profiles.Any(p => p.Name == profile.Name))
            {
                return Result.Failure($"Profile with name '{profile.Name}' already exists.");
            }

            _profiles.Add(profile);
            _logger.Debug($"Added new profile '{profile.Name}'");
            return Result.Success();
        }

        public Result<Profile> GetProfile(string profileName)
        {
            var profile = _profiles.FirstOrDefault(p => p.Name == profileName);
            return profile == null
                ? Result.Failure<Profile>($"Profile '{profileName}' not found")
                : Result.Success(profile);
        }

        public string[] GetProfileNames()
        {
            return _profiles
                .Select(profile => profile.Name)
                .ToArray();
        }

        public Result RemoveProfile(Profile profile)
        {
            var profileToRemove = _profiles.FirstOrDefault(p => p.Name == profile.Name);
            if (profileToRemove == null)
            {
                return Result.Failure($"Profile '{profile.Name}' not found");
            }

            _profiles.Remove(profileToRemove);
            _profileSaver.SaveProfiles(_profiles.ToArray());
            return Result.Success();
        }

        public Result Update(Profile selectedProfile)
        {
            var profileToUpdate = _profiles.FirstOrDefault(p => p.Name == selectedProfile.Name);
            if (profileToUpdate == null)
            {
                return Result.Failure($"Profile '{selectedProfile.Name}' not found");
            }

            profileToUpdate.HostName = selectedProfile.HostName;
            profileToUpdate.HostPort = selectedProfile.HostPort;
            profileToUpdate.UserName = selectedProfile.UserName;
            profileToUpdate.UserPassword = selectedProfile.UserPassword;

            _profileSaver.SaveProfiles(_profiles.ToArray());
            return Result.Success();
        }

        private void LoadProfiles()
        {
            var profilesResult = _profileLoader.LoadProfiles();
            if (profilesResult.IsSuccess)
            {
                _profiles = profilesResult.Value;
                return;
            }

            _logger.Debug($"Profiles.json not found. Creating default profiles.");
            var defaultProfile = new Profile
            {
                Name = "Local",
                HostName = "localhost",
                HostPort = 9050,
                UserName = "admin",
                UserPassword = "admin"
            };
            _profiles = new List<Profile>
            {
                defaultProfile
            };
            _profileSaver.SaveProfiles(_profiles.ToList());
        }
    }
}
