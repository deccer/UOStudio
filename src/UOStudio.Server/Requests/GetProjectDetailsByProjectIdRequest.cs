using CSharpFunctionalExtensions;
using LiteNetLib.Utils;
using MediatR;
using UOStudio.Common.Contracts;

namespace UOStudio.Server.Requests
{
    public readonly struct GetProjectDetailsByProjectIdRequest : IRequest<Result<ProjectDetailDto>>
    {
        public GetProjectDetailsByProjectIdRequest(NetDataReader reader)
        {
            UserId = reader.GetInt();
            ProjectId = reader.GetInt();
        }

        public int UserId { get; }

        public int ProjectId { get; }
    }
}
