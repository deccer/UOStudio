using CSharpFunctionalExtensions;
using LiteNetLib.Utils;
using MediatR;
using UOStudio.Common.Contracts;

namespace UOStudio.Server.Requests
{
    public readonly struct GetProjectDetailsByProjectNameRequest : IRequest<Result<ProjectDetailDto>>
    {
        public GetProjectDetailsByProjectNameRequest(NetDataReader reader)
        {
            UserId = reader.GetInt();
            ProjectName = reader.GetString();
        }

        public int UserId { get; }

        public string ProjectName { get; }
    }
}
