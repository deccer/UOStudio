using CSharpFunctionalExtensions;

namespace UOStudio
{
    public interface IProfileLoader
    {
        Result<Profile[]> LoadProfiles();
    }
}
