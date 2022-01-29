using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Serilog;
using UOStudio.Common.Contracts;
using UOStudio.Server.Data;

namespace UOStudio.Server.Domain.GetUserById
{
    [UsedImplicitly]
    internal sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
    {
        private readonly ILogger _logger;
        private readonly ILiteDbFactory _liteDbFactory;

        public GetUserByIdQueryHandler(
            ILogger logger,
            ILiteDbFactory liteDbFactory)
        {
            _logger = logger;
            _liteDbFactory = liteDbFactory;
        }

        public Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
