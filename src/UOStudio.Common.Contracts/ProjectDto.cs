namespace UOStudio.Common.Contracts
{
    public record ProjectDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
