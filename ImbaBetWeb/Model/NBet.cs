namespace ImbaBetWeb.Model
{
    public record NBet
    {
        public int Id { get; set; }

        public int MatchId { get; set; }

        public int UserId { get; set; }

        public int GoalsA { get; set; }

        public int GoalsB { get; set; }
        public int Points { get; set; }
    }
}
