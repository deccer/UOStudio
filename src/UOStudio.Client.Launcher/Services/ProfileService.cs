using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UOStudio.Client.Launcher.Contracts;
using UOStudio.Client.Launcher.Data;

namespace UOStudio.Client.Launcher.Services
{
    public sealed class ProfileService : IProfileService
    {
        private readonly ILogger _logger;
        private readonly IDbContextFactory<ProfileDbContext> _contextFactory;

        public ProfileService(
            ILogger logger,
            IDbContextFactory<ProfileDbContext> contextFactory)
        {
            _logger = logger.ForContext<ProfileService>();
            _contextFactory = contextFactory;
        }

        public IReadOnlyCollection<ProfileNameAndDescriptionDto> GetProfilesWithNameAndDescription()
        {
            using var context = _contextFactory.CreateDbContext();
            var profiles = context.Profiles
                .AsNoTracking()
                .Select(p => new ProfileNameAndDescriptionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            }).AsEnumerable();

            return profiles.ToImmutableList();
        }

        public IReadOnlyCollection<LookupItem> GetProfiles()
        {
            using var context = _contextFactory.CreateDbContext();
            var profiles = context.Profiles
                .AsNoTracking()
                .Select(profile => new LookupItem
                {
                    Id = profile.Id,
                    Name = profile.Name
                }).AsEnumerable();

            return profiles.ToImmutableList();
        }

        public async Task<ProfileDto> GetProfileAsync(int profileId)
        {
            using var context = _contextFactory.CreateDbContext();
            var profile = await context.Profiles
                .AsNoTracking()
                .Where(p => p.Id == profileId)
                .Select(profile => new ProfileDto
                {
                    Id = profile.Id,
                    Name = profile.Name,
                    Description = profile.Description,
                    ServerName = profile.ServerName,
                    ServerPort = profile.ServerPort,
                    UserName = profile.UserName,
                    Password = profile.Password,
                    ApiBaseUri = profile.ApiBaseUri,
                    AuthBaseUri = profile.AuthBaseUri
                })
                .SingleOrDefaultAsync();
            return profile;
        }

        public async Task DeleteProfileAsync(int profileId)
        {
            using var context = _contextFactory.CreateDbContext();
            var profile = await context.Profiles.FindAsync(profileId);
            if (profile != null)
            {
                context.Profiles.Remove(profile);
            }

            await context.SaveChangesAsync();
        }

        public async Task UpdateProfileAsync(int profileId, ProfileDto profileDto)
        {
            using var context = _contextFactory.CreateDbContext();
            var profile = await context.Profiles.FindAsync(profileId);
            if (profile != null)
            {
                profile.Name = profileDto.Name;
                profile.Description = profileDto.Description;
                profile.ServerName = profileDto.ServerName;
                profile.ServerPort = profileDto.ServerPort;
                profile.UserName = profileDto.UserName;
                profile.Password = profileDto.Password;
                profile.AuthBaseUri = profileDto.AuthBaseUri;
                profile.ApiBaseUri = profileDto.ApiBaseUri;

                await context.SaveChangesAsync();
            }
        }

        public async Task<Result> AddProfileAsync(ProfileDto profileDto)
        {
            using var context = _contextFactory.CreateDbContext();
            var profile = await context.Profiles.FindAsync(profileDto.Id);
            if (profile != null)
            {
                return Result.Failure("Profile already exists");
            }

            profile = new Profile
            {
                Name = profileDto.Name,
                Description = profileDto.Description,
                ServerName = profileDto.ServerName,
                ServerPort = profileDto.ServerPort,
                UserName = profileDto.UserName,
                Password = profileDto.Password,
                AuthBaseUri = profileDto.AuthBaseUri,
                ApiBaseUri = profileDto.ApiBaseUri
            };

            await context.Profiles.AddAsync(profile);
            return Result.Success();
        }
    }
}