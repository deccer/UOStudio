using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;

namespace UOStudio.Server.Requests
{
    [UsedImplicitly]
    internal sealed class CreateProjectRequestHandler : IRequestHandler<CreateProjectRequest, Result<int>>
    {
        public Task<Result<int>> Handle(CreateProjectRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
