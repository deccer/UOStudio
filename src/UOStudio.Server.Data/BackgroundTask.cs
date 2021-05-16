using System;

namespace UOStudio.Server.Data
{
    public class BackgroundTask
    {
        public Guid Id { get; set; }

        public BackgroundTaskStatus Status { get; set; }

        public string CompletedLocation { get; set; }

        public string ErrorMessage { get; set; }
    }
}
