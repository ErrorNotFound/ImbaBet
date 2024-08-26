
using ImbaBetWeb.Models;

namespace ImbaBetWeb.ViewModels.Admin
{
    public class MatchesViewModel
    {
        public IList<MatchGroup> MatchGroups { get; set; }

        public IList<Match> Matches { get; set; }

        public IList<Team> Teams { get; set; }
    }
}
