using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using UOStudio.Common.Contracts;

namespace UOStudio.Server.Api.Services
{
    public interface IUserService
    {
        Task<Result> ValidateCredentialsAsync(UserCredentials userCredentials, CancellationToken cancellationToken = default);

        Task<Result> UpdateConnectionTicketAsync(string userName, string connectionTicket, CancellationToken cancellationToken = default);

        Task<bool> ValidateConnectionTicketAsync(string connectionTicketDecoded);
    }
}
