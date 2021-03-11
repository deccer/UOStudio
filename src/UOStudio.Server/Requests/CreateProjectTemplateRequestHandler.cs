using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;

namespace UOStudio.Server.Requests
{
    [UsedImplicitly]
    internal sealed class CreateProjectTemplateRequestHandler : IRequestHandler<CreateProjectTemplateRequest, Result<int>>
    {
        public async Task<Result<int>> Handle(CreateProjectTemplateRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
