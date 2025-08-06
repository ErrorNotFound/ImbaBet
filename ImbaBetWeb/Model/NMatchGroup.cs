namespace ImbaBetWeb.Model
{
    public record NMatchGroup
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public bool HasGroupRanking { get; set; }

        public int StackRank { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }
}
