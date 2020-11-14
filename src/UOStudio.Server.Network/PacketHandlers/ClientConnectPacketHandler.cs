using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Serilog;
using UOStudio.Server.Core;

namespace UOStudio.Server.Network.PacketHandlers
{
    public class ClientConnectPacketHandler : IPacketHandler<ClientConnectPacket, int>
    {
        private readonly ILogger _logger;
        private readonly IAccountStore _accountStore;

        public ClientConnectPacketHandler(ILogger logger, IAccountStore accountStore)
        {
            _logger = logger.ForContext<ClientConnectPacketHandler>();
            _accountStore = accountStore;
        }

        public Task<Result<int>> Handle(ClientConnectPacket clientConnectPacket)
        {
            var account = _accountStore.GetAccount(clientConnectPacket.UserName);
            if (account == null)
            {
                return Task.FromResult(Result.Failure<int>("Account not found."));
            }

            if (account.IsBlocked())
            {
                var message = $"Blocked user {clientConnectPacket.UserName} tried to log in.";
                _logger.Warning(message);
                return Task.FromResult(Result.Failure<int>(message));
            }

            return Task.FromResult(Result.Success(42));
        }
    }
}
