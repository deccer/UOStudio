using CSharpFunctionalExtensions;

namespace UOStudio.Server.Services
{
    public interface IGitClient
    {
        Result Clone(string repository, string localRepositoryPath);

        Result CloneBranch(string repository, string branchName, string repositoryPath);
    }
}
