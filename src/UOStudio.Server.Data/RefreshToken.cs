namespace UOStudio.Server.Data
{
    public record RefreshToken
    {
        public int Id { get; set; }

        public string Value { get; set; }
    }
}
