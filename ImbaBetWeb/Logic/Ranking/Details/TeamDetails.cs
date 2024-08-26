using ImbaBetWeb.Models;

namespace ImbaBetWeb.Logic.Ranking.Details
{
    public class TeamDetails
    {
        public Team Team { get; set; }
        public int MatchesPlayed { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Lost { get; set; }
        public int Goals { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference { get { return Goals - GoalsAgainst; } }
    }
}
