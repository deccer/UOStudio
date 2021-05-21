using System;

namespace UOStudio.Server.Api.BackgroundJobs
{
    internal class Job
    {
        public Guid Id { get; set; }

        public string JobType { get; set; }

        public string RequestPayload { get; set; }

        public string RequestPayloadType { get; set; }

        public JobStatus Status { get; set; }

        public DateTime StatusDate { get; set; }

        public string ResultType { get; set; }

        public string ResultPayload { get; set; }
    }
}
