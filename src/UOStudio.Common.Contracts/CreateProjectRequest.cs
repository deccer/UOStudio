namespace UOStudio.Common.Contracts
{
    public class CreateProjectRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int TemplateId { get; set; }
    }
}
