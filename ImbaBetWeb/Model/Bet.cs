namespace ImbaBetWeb.Model
{
    public record Bet
    {
        public int Id { get; set; }

        public Match? Match { get; set; } = null!;
        public int MatchId { get; set; }

        public BettingUser? User { get; set; } = null!;
        public int UserId { get; set; }

        public int GoalsA { get; set; }

        public int GoalsB { get; set; }
        public int Points { get; set; }
    }
}
