using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using UOStudio.Client.Launcher.Contracts;

namespace UOStudio.Client.Launcher.Services
{
    public interface ITokenClient
    {
        Task<Result<TokenPair>> AcquireTokenPairAsync(CancellationToken cancellationToken = default);
    }
}