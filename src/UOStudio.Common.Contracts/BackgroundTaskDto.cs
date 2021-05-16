using System;

namespace UOStudio.Common.Contracts
{
    public class BackgroundTaskDto
    {
        public Guid Id { get; set; }

        public BackgroundTaskStatus Status { get; set; }

        public string CompletedLocation { get; set; }

        public string ErrorMessage { get; set; }
    }
}
