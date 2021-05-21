using System;

namespace UOStudio.Server.Api.BackgroundJobs
{
    public class JobDto
    {
        public JobDto(Guid id, string jobType, string requestPayload, string requestPayloadType)
        {
            Id = id;
            JobType = jobType;
            RequestPayload = requestPayload;
            RequestPayloadType = requestPayloadType;
        }

        public Guid Id { get; }

        public string JobType { get; }

        public string RequestPayload { get; }

        public string RequestPayloadType { get; }
    }
}
