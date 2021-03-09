using CSharpFunctionalExtensions;
using LiteNetLib.Utils;
using MediatR;

namespace UOStudio.Server.Requests
{
    public readonly struct CreateProjectRequest : IRequest<Result<int>>
    {
        public CreateProjectRequest(NetDataReader reader)
        {
            UserId = reader.GetInt();
            Name = reader.GetString();
            Description = reader.GetString();
            TemplateId = reader.GetInt();
            IsPublic = reader.GetBool();
        }

        public int UserId { get; }

        public string Name { get; }

        public string Description { get; }

        public int TemplateId { get; }

        public bool IsPublic { get; }
    }
}
