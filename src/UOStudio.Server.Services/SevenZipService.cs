using System.IO;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;

namespace UOStudio.Server.Services
{
    public sealed class SevenZipService : ISevenZipService
    {
        private readonly SevenZipSettings _sevenZipOptions;
        private readonly ICommandRunner _commandRunner;

        public SevenZipService(
            ICommandRunner commandRunner,
            IOptions<SevenZipSettings> sevenZipOptions)
        {
            _sevenZipOptions = sevenZipOptions.Value;
            _commandRunner = commandRunner;
        }

        public Result<bool> Zip(string archiveFileName, string sourceDirectory)
        {
            var runResult = _commandRunner.RunCommand(_sevenZipOptions.Location, $"a -t7z -mx9 {archiveFileName} {sourceDirectory}", Path.GetDirectoryName(archiveFileName));
            return runResult.IsSuccess
                ? Result.Success(true)
                : Result.Failure<bool>(runResult.Error);
        }

        public Result<bool> Unzip(string archiveFileName, string targetDirectory)
        {
            var runResult = _commandRunner.RunCommand(_sevenZipOptions.Location, $"e {archiveFileName} -o{targetDirectory}", Path.GetDirectoryName(archiveFileName));
            return runResult.IsSuccess
                ? Result.Success(true)
                : Result.Failure<bool>(runResult.Error);
        }
    }
}
