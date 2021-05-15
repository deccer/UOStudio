using CSharpFunctionalExtensions;
using UOStudio.Client.Launcher.Contracts;

namespace UOStudio.Client.Launcher.Services
{
    public interface IClientStarterService
    {
        Result StartClientAsync(ProjectDto project);
    }
}
