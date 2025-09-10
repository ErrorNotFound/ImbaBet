namespace ImbaBetWeb.Model
{
    public class Matchplan
    {
        public required IEnumerable<MatchGroup> MatchGroups { get; set; }
        public required IEnumerable<Match> Matches { get; set; }
        public required IEnumerable<Team> Teams { get; set; }
    }
}
