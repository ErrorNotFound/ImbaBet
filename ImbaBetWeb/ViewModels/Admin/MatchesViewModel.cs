using ImbaBetWeb.Model;

namespace ImbaBetWeb.ViewModels.Admin
{
    public class MatchesViewModel
    {
        public required List<Match> Matches { get; set; }
        public required List<MatchGroup> MatchGroups { get; set; }
        public required List<Team> Teams { get; set; }
    }
}
