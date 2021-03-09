using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Serilog;

namespace UOStudio.Server.Requests
{
    [UsedImplicitly]
    internal sealed class ClientLogoutRequestHandler : IRequestHandler<ClientLogoutRequest, Result<int>>
    {
        private readonly ILogger _logger;

        public ClientLogoutRequestHandler(
            ILogger logger)
        {
            _logger = logger;
        }

        public Task<Result<int>> Handle(ClientLogoutRequest request, CancellationToken cancellationToken)
        {
            _logger.Debug("{@TypeName}", GetType().Name);
            return Task.FromResult(Result.Success(0));
        }
    }
}
