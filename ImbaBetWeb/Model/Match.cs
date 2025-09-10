namespace ImbaBetWeb.Model
{
    public record Match : IIdentifiable<int>
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public Team? TeamA { get; set; } = null!;
        public int? TeamATeamId { get; set; }
        public Team? TeamB { get; set; } = null!;
        public int? TeamBTeamId { get; set; }

        public string? AlternativeTeamAText { get; set; }

        public string? AlternativeTeamBText { get; set; }

        public int GoalsA { get; set; }

        public int GoalsB { get; set; }

        public bool IsOver { get; set; }

        public MatchGroup? MatchGroup { get; set; } = null!;
        public int MatchGroupId { get; set; }

        public override string ToString()
        {
            return $"Match ({Id})";
        }
    }
}
