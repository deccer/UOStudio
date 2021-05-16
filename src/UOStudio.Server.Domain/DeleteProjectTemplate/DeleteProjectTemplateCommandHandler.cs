using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UOStudio.Server.Data;

namespace UOStudio.Server.Domain.DeleteProjectTemplate
{
    [UsedImplicitly]
    internal sealed class DeleteProjectTemplateCommandHandler : IRequestHandler<DeleteProjectTemplateCommand, Result>
    {
        private readonly ILogger _logger;
        private readonly IDbContextFactory<UOStudioContext> _contextFactory;

        public DeleteProjectTemplateCommandHandler(
            ILogger logger,
            IDbContextFactory<UOStudioContext> contextFactory)
        {
            _logger = logger.ForContext<DeleteProjectTemplateCommandHandler>();
            _contextFactory = contextFactory;
        }

        public Task<Result> Handle(DeleteProjectTemplateCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
