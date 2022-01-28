using CSharpFunctionalExtensions;
using UOStudio.Client.Launcher.Contracts;

namespace UOStudio.Client.Launcher.Data.Repositories
{
    public interface IProfileRepository
    {
        IReadOnlyCollection<ProfileNameAndDescriptionDto> GetProfilesWithNameAndDescription();

        IReadOnlyCollection<LookupItem> GetProfiles();

        Task<ProfileDto> GetProfileAsync(int profileId);

        Task DeleteProfileAsync(int profileId);

        Task UpdateProfileAsync(int profileId, ProfileDto profileDto);

        Task<Result> AddProfileAsync(ProfileDto profileDto);
    }
}
