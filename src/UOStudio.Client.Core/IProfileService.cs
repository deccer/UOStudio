using CSharpFunctionalExtensions;

namespace UOStudio
{
    public interface IProfileService
    {
        Result AddProfile(Profile profile);

        Result<Profile> GetProfile(string profileName);

        string[] GetProfileNames();

        Result RemoveProfile(Profile profile);

        Result Update(Profile selectedProfile);
   }
}
