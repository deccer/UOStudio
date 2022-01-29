using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Serilog;
using UOStudio.Server.Data;

namespace UOStudio.Server.Domain.DeleteProjectTemplate
{
    [UsedImplicitly]
    internal sealed class DeleteProjectTemplateCommandHandler : IRequestHandler<DeleteProjectTemplateCommand, Result>
    {
        private readonly ILogger _logger;
        private readonly ILiteDbFactory _liteDbFactory;

        public DeleteProjectTemplateCommandHandler(
            ILogger logger,
            ILiteDbFactory liteDbFactory)
        {
            _logger = logger.ForContext<DeleteProjectTemplateCommandHandler>();
            _liteDbFactory = liteDbFactory;
        }

        public Task<Result> Handle(DeleteProjectTemplateCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
