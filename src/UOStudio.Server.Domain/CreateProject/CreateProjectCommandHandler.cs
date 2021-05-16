using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UOStudio.Server.Data;

namespace UOStudio.Server.Domain.CreateProject
{
    [UsedImplicitly]
    internal sealed class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Result<int>>
    {
        private readonly ILogger _logger;
        private readonly IDbContextFactory<UOStudioContext> _contextFactory;

        public CreateProjectCommandHandler(
            ILogger logger,
            IDbContextFactory<UOStudioContext> contextFactory)
        {
            _logger = logger.ForContext<CreateProjectCommandHandler>();
            _contextFactory = contextFactory;
        }

        public Task<Result<int>> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
        {
            return Task.FromResult(Result.Success(0));
        }
    }
}
