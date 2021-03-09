using CSharpFunctionalExtensions;
using LiteNetLib.Utils;
using MediatR;

namespace UOStudio.Server.Requests
{
    public readonly struct ClientLoginRequest : IRequest<Result<ClientLoginResult>>
    {
        public ClientLoginRequest(NetDataReader reader)
        {
            UserName = reader.GetString();
            Password = reader.GetString();
        }

        public string UserName { get; }

        public string Password { get; }
    }
}
