using System;

namespace UOStudio.Web.Data.Entities
{
    public class TemporaryFile
    {
        public Guid Id { get; set; }

        public Guid ProjectId { get; set; }

        public Guid FileId { get; set; }
    }
}
