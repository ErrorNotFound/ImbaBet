namespace ImbaBetWeb.Model
{
    public record NSetting
    {
        public required string Key { get; set; }
        public required string Value { get; set; }
        public required string Default { get; set; }
        public required string Description { get; set; }
    }
}
