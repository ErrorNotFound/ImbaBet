namespace ImbaBetWeb.Model
{
    public record Setting : IIdentifiable<string>
    {
        public required string Id { get; set; }
        public required string Value { get; set; }
        public required string Default { get; set; }
        public required string Description { get; set; }
    }
}
