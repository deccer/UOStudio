using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using UOStudio.Common.Contracts;
using UOStudio.Server.Api.Models;

namespace UOStudio.Server.Api.Services
{
    public interface IUserService
    {
        Task<Result<Tokens>> LoginAsync(AuthenticationRequest authenticationRequest, CancellationToken cancellationToken);

        Task<Result> RefreshAsync(int userId, string refreshToken, CancellationToken cancellationToken);
    }
}
