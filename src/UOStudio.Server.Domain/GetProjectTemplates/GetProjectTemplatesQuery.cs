using System.Collections.Generic;
using System.Security.Principal;
using CSharpFunctionalExtensions;
using MediatR;
using UOStudio.Common.Contracts;
using UOStudio.Common.Core.Extensions;

namespace UOStudio.Server.Domain.GetProjectTemplates
{
    public sealed class GetProjectTemplatesQuery : IRequest<Result<IList<ProjectTemplateDto>>>
    {
        public GetProjectTemplatesQuery(IPrincipal principal)
        {
            User = principal;
        }

        public IPrincipal User { get; }
    }
}
