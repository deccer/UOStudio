using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Serilog;
using UOStudio.Server.Data;

namespace UOStudio.Server.Domain.CreateProject
{
    [UsedImplicitly]
    internal sealed class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Result<int>>
    {
        private readonly ILogger _logger;
        private readonly ILiteDbFactory _liteDbFactory;

        public CreateProjectCommandHandler(
            ILogger logger,
            ILiteDbFactory liteDbFactory)
        {
            _logger = logger.ForContext<CreateProjectCommandHandler>();
            _liteDbFactory = liteDbFactory;
        }

        public Task<Result<int>> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
        {
            return Task.FromResult(Result.Success(0));
        }
    }
}
