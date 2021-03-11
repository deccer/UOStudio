using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using UOStudio.Common.Contracts;

namespace UOStudio.Server.Requests
{
    [UsedImplicitly]
    internal sealed class GetProjectDetailsByProjectNameRequestHandler : IRequestHandler<GetProjectDetailsByProjectNameRequest, Result<ProjectDetailDto>>
    {
        public async Task<Result<ProjectDetailDto>> Handle(GetProjectDetailsByProjectNameRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
