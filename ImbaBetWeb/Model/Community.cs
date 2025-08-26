namespace ImbaBetWeb.Model
{
    public record Community : IIdentifiable<int>
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public required int OwnerId { get; set; }
        public BettingUser? Owner { get; set; } = null!;

        public IEnumerable<BettingUser> Members { get; set; } = null!;

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
