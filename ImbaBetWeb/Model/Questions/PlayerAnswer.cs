namespace ImbaBetWeb.Model.Questions
{
    public record PlayerAnswer : IIdentifiable<int>
    {
        public int Id { get; set; }

        public Question Question { get; set; } = null!;
        public int QuestionId { get; set; }

        public Player? Player { get; set; } = null!;
        public int PlayerId { get; set; }

        public required string Answer { get; set; }
    }
}
