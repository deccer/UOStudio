using CSharpFunctionalExtensions;
using LiteNetLib.Utils;
using MediatR;

namespace UOStudio.Server.Requests
{
    public readonly struct ClientLogoutRequest : IRequest<Result<int>>
    {
        public ClientLogoutRequest(NetDataReader reader)
            => UserId = reader.GetInt();

        public int UserId { get; }
    }
}
