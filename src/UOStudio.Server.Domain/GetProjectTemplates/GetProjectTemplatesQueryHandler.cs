using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UOStudio.Common.Contracts;
using UOStudio.Server.Data;

namespace UOStudio.Server.Domain.GetProjectTemplates
{
    [UsedImplicitly]
    internal sealed class GetProjectTemplatesQueryHandler : IRequestHandler<GetProjectTemplatesQuery, Result<IList<ProjectTemplateDto>>>
    {
        private readonly ILogger _logger;
        private readonly IDbContextFactory<UOStudioContext> _contextFactory;

        public GetProjectTemplatesQueryHandler(
            ILogger logger,
            IDbContextFactory<UOStudioContext> contextFactory)
        {
            _logger = logger.ForContext<GetProjectTemplatesQueryHandler>();
            _contextFactory = contextFactory;
        }

        public async Task<Result<IList<ProjectTemplateDto>>> Handle(
            GetProjectTemplatesQuery request,
            CancellationToken cancellationToken)
        {
            await using var db = _contextFactory.CreateDbContext();

            var projectTemplates = db.ProjectTemplates
                .AsNoTracking()
                .AsQueryable().Select(pt => new ProjectTemplateDto
                {
                    Id = pt.Id,
                    Name = pt.Name
                }).ToList();

            return Result.Success<IList<ProjectTemplateDto>>(projectTemplates);
        }
    }
}
