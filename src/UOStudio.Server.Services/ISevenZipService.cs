using CSharpFunctionalExtensions;

namespace UOStudio.Server.Services
{
    public interface ISevenZipService
    {
        Result<bool> Zip(string archiveFileName, string sourceDirectory);

        Result<bool> Unzip(string archiveFileName, string targetDirectory);
    }
}
