using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;

namespace UOStudio.Server.Requests
{
    [UsedImplicitly]
    internal sealed class ClientLoginRequestHandler : IRequestHandler<ClientLoginRequest, Result<ClientLoginResult>>
    {
        public Task<Result<ClientLoginResult>> Handle(ClientLoginRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
