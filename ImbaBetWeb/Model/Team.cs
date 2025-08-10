namespace ImbaBetWeb.Model
{
    public record Team
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public string? FlagCountryCode { get; set; }

        public int StackRank { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }
}
