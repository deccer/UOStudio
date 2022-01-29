using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Serilog;
using UOStudio.Common.Contracts;
using UOStudio.Server.Data;

namespace UOStudio.Server.Domain.GetProjectTemplates
{
    [UsedImplicitly]
    internal sealed class GetProjectTemplatesQueryHandler : IRequestHandler<GetProjectTemplatesQuery, Result<IList<ProjectTemplateDto>>>
    {
        private readonly ILogger _logger;
        private readonly ILiteDbFactory _liteDbFactory;

        public GetProjectTemplatesQueryHandler(
            ILogger logger,
            ILiteDbFactory liteDbFactory)
        {
            _logger = logger.ForContext<GetProjectTemplatesQueryHandler>();
            _liteDbFactory = liteDbFactory;
        }

        public async Task<Result<IList<ProjectTemplateDto>>> Handle(
            GetProjectTemplatesQuery request,
            CancellationToken cancellationToken)
        {
            using var db = _liteDbFactory.CreateLiteDatabase();

            var projectTemplates = (await db
                    .GetCollection<ProjectTemplate>()
                    .FindAllAsync())
                .Select(pt => new ProjectTemplateDto
                {
                    Id = pt.Id,
                    Name = pt.Name
                }).ToList();

            return Result.Success<IList<ProjectTemplateDto>>(projectTemplates);
        }
    }
}
