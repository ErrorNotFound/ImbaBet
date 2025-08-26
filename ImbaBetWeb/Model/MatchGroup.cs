namespace ImbaBetWeb.Model
{
    public record MatchGroup : IIdentifiable<int>
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public bool HasGroupRanking { get; set; }

        public int StackRank { get; set; }

        public IEnumerable<Match> Matches { get; set; } = null!;

        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }
}
