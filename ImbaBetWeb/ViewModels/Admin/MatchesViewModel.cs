
using ImbaBetWeb.Models;

namespace ImbaBetWeb.ViewModels.Admin
{
    public class MatchesViewModel
    {
        public required IList<MatchGroup> MatchGroups { get; set; }

        public required IList<Match> Matches { get; set; }

        public required IList<Team> Teams { get; set; }
    }
}
