namespace ImbaBetWeb.Model
{
    public record Bet : IIdentifiable<int>
    {
        public int Id { get; set; }

        public Match? Match { get; set; } = null!;
        public int MatchId { get; set; }

        public Player? Player { get; set; } = null!;
        public int PlayerId { get; set; }

        public int GoalsA { get; set; }

        public int GoalsB { get; set; }
        public int Points { get; set; }
    }
}
