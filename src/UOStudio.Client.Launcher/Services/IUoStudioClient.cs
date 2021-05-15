using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using UOStudio.Client.Launcher.Contracts;

namespace UOStudio.Client.Launcher.Services
{
    public interface IUoStudioClient
    {
        Task<Result<IImmutableList<ProjectDto>>> GetProjectsAsync(CancellationToken cancellationToken = default);
    }
}