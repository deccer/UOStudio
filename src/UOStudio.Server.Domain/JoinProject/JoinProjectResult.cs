using System;

namespace UOStudio.Server.Domain.JoinProject
{
    public struct JoinProjectResult
    {
        public bool NeedsPreparation { get; set; }

        public Guid? TaskId { get; set; }
    }
}
