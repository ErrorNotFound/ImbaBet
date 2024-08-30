using ImbaBetWeb.Logic.Extensions;
using ImbaBetWeb.Logic.Ranking.Details;
using ImbaBetWeb.Models;

namespace ImbaBetWeb.Logic.Ranking.Comparing
{
    public class GroupRankingComparer : IComparer<RankingItem<TeamDetails>>
    {
        private MatchGroup _matchGroup;

        public GroupRankingComparer(MatchGroup mg) 
        {
            _matchGroup = mg;
        }

        public int Compare(RankingItem<TeamDetails>? x, RankingItem<TeamDetails>? y)
        {
            if (x == null && y == null)
                return 0;
            else if (x == null)
                return 1;
            else if (y == null)
                return -1;

            // StackRank shall always win
            var byStackRank = CompareByStackRank(x, y);
            if (byStackRank != 0)
            {
                return byStackRank;
            }

            var byPoints = CompareByPointsDescending(x, y);
            if (byPoints != 0)
            {
                return byPoints;
            }

            var byDirectCompare = CompareByPointsDescending(x, y);
            if (byDirectCompare != 0)
            {
                return byDirectCompare;
            }

            return CompareByGoalDifferenceDescending(x, y);
        }

        private int CompareByPointsDescending(RankingItem<TeamDetails> x, RankingItem<TeamDetails> y)
        {
            return y.Points - x.Points;
        }

        private int CompareByGoalDifferenceDescending(RankingItem<TeamDetails> x, RankingItem<TeamDetails> y)
        {
            return y.Details.GoalDifference - x.Details.GoalDifference;
        }

        private int CompareByDirectMatchDescending(RankingItem<TeamDetails> x, RankingItem<TeamDetails> y)
        {
            var directMatch = _matchGroup.Matches.SingleOrDefault(m =>
            m.IsOver
            && (m.TeamA == x.Details.Team || m.TeamA == y.Details.Team)
            && (m.TeamB == x.Details.Team || m.TeamA == y.Details.Team));

            if (directMatch == null || directMatch.HasTeamDrawed(x.Details.Team))
                return 0;

            return directMatch.HasTeamWon(x.Details.Team) ? -1 : 1;
        }

        private int CompareByStackRank(RankingItem<TeamDetails> x, RankingItem<TeamDetails> y)
        {
            return x.Details.Team.StackRank - y.Details.Team.StackRank;
        }

        
    }
}
