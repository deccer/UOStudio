using CSharpFunctionalExtensions;
using LiteNetLib.Utils;
using MediatR;

namespace UOStudio.Server.Requests
{
    public readonly struct CreateProjectTemplateRequest : IRequest<Result<int>>
    {
        public CreateProjectTemplateRequest(NetDataReader reader)
        {
            UserId = reader.GetInt();
            TemplateName = reader.GetString();
            ClientVersion = reader.GetString();
        }

        public int UserId { get; }

        public string TemplateName { get; }

        public string ClientVersion { get; }
    }
}
