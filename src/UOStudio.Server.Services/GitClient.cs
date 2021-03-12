using System.IO;
using CSharpFunctionalExtensions;
using Serilog;

namespace UOStudio.Server.Services
{
    public sealed class GitClient : IGitClient
    {
        private const string Git = "git";
        private const string CloneArgument = "clone --recursive {0} {1}";
        private const string CloneBranchArgument = "clone --branch {0} --single-branch --recursive {1} {2}";

        private readonly ILogger _logger;
        private readonly ICommandRunner _commandRunner;

        public GitClient(
            ILogger logger,
            ICommandRunner commandRunner
        )
        {
            _logger = logger;
            _commandRunner = commandRunner;
        }

        public Result Clone(string repository, string localRepositoryPath)
        {
            if (Directory.Exists(localRepositoryPath))
            {
                return Result.Failure<string>($"Directory {localRepositoryPath} already exists");
            }

            var arguments = string.Format(CloneArgument, repository, localRepositoryPath);
            var commandResult = _commandRunner.RunCommand(Git, arguments, null);
            return commandResult.IsSuccess
                ? Result.Success()
                : Result.Failure($"Unable to clone repo {repository} to {localRepositoryPath}");
        }

        public Result CloneBranch(string repository, string branchName, string repositoryPath)
        {
            if (Directory.Exists(repositoryPath))
            {
                return Result.Failure<string>($"Directory {repositoryPath} already exists");
            }

            var arguments = string.Format(CloneBranchArgument, branchName, repository, repositoryPath);
            var commandResult = _commandRunner.RunCommand(Git, arguments, null);
            return commandResult.IsSuccess
                ? Result.Success()
                : Result.Failure($"Unable to clone repo {repository} to {repositoryPath}");
        }
    }
}
