namespace ImbaBetWeb.Model
{
    public record NMatch
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public int? TeamATeamId { get; set; }

        public int? TeamBTeamId { get; set; }

        public string? AlternativeTeamAText { get; set; }

        public string? AlternativeTeamBText { get; set; }

        public int GoalsA { get; set; }

        public int GoalsB { get; set; }

        public bool IsOver { get; set; }

        public int MatchGroupId { get; set; }

        public override string ToString()
        {
            return $"Match ({Id})";
        }
    }
}
