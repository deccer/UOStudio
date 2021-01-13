using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace UOStudio
{
    public interface IProfileSaver
    {
        void SaveProfiles(IReadOnlyCollection<Profile> profiles);
    }
}
