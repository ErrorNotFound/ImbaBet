
using ImbaBetWeb.Model;

namespace ImbaBetWeb.ViewModels.Admin
{
    public class MatchesViewModel
    {
        public required IEnumerable<MatchGroup> MatchGroups { get; set; }

        public required IEnumerable<Match> Matches { get; set; }

        public required IEnumerable<Team> Teams { get; set; }
    }
}
