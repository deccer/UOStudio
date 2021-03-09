using CSharpFunctionalExtensions;

namespace UOStudio.Server.Services
{
    public interface ICommandRunner
    {
        Result<string> RunCommand(string command, string arguments, string workDirectory);
    }
}
