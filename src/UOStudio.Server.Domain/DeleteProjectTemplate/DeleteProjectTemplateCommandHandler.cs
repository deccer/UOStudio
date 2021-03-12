using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UOStudio.Server.Data;

namespace UOStudio.Server.Domain.DeleteProjectTemplate
{
    public sealed class DeleteProjectTemplateCommandHandler : IRequestHandler<DeleteProjectTemplateCommand, Result>
    {
        private readonly ILogger _logger;
        private readonly IDbContextFactory<UOStudioContext> _contextFactory;

        public DeleteProjectTemplateCommandHandler(
            ILogger logger,
            IDbContextFactory<UOStudioContext> contextFactory)
        {
            _logger = logger;
            _contextFactory = contextFactory;
        }

        public Task<Result> Handle(DeleteProjectTemplateCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
