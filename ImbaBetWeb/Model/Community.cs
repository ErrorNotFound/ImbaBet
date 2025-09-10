namespace ImbaBetWeb.Model
{
    public record Community : IIdentifiable<int>
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public required int OwnerId { get; set; }
        public Player? Owner { get; set; } = null!;

        public IEnumerable<Player> Members { get; set; } = null!;

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
