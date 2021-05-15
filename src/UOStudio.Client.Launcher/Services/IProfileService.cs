using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using UOStudio.Client.Launcher.Contracts;

namespace UOStudio.Client.Launcher.Services
{
    public interface IProfileService
    {
        IReadOnlyCollection<ProfileNameAndDescriptionDto> GetProfilesWithNameAndDescription();

        IReadOnlyCollection<LookupItem> GetProfiles();

        Task<ProfileDto> GetProfileAsync(int profileId);

        Task DeleteProfileAsync(int profileId);

        Task UpdateProfileAsync(int profileId, ProfileDto profileDto);

        Task<Result> AddProfileAsync(ProfileDto profileDto);
    }
}