using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UOStudio.Server.Data;

namespace UOStudio.Server.Network.PacketHandlers
{
    public class ClientConnectRequestHandler : IRequestHandler<ClientConnectRequest, ClientConnectResult>
    {
        private readonly ILogger _logger;
        private readonly IDbContextFactory<UOStudioDbContext> _contextFactory;

        public ClientConnectRequestHandler(ILogger logger, IDbContextFactory<UOStudioDbContext> contextFactory)
        {
            _logger = logger.ForContext<ClientConnectRequestHandler>();
            _contextFactory = contextFactory;
        }

        public async Task<Result<ClientConnectResult>> Handle(ClientConnectRequest clientConnectRequest)
        {
            await using var context = _contextFactory.CreateDbContext();
            var account = await context.Accounts.FirstOrDefaultAsync();
            if (account == null)
            {
                return await Task.FromResult(Result.Failure<ClientConnectResult>("Account not found."));
            }

            if (account.IsBlocked())
            {
                var message = $"Blocked user {clientConnectRequest.UserName} tried to log in.";
                _logger.Warning(message);
                return await Task.FromResult(Result.Failure<ClientConnectResult>(message));
            }

            var projects = await context.Projects.ToListAsync();

            return await Task.FromResult(Result.Success(new ClientConnectResult(account.Id, projects)));
        }
    }
}
