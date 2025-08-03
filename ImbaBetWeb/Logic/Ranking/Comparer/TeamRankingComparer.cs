using ImbaBetWeb.Logic.Ranking.Details;

namespace ImbaBetWeb.Logic.Ranking.Comparer
{
    public class TeamRankingComparer : IComparer<RankingItem<TeamDetails>>
    {
        public int Compare(RankingItem<TeamDetails>? x, RankingItem<TeamDetails>? y)
        {
            if (x == null && y == null)
                return 0;
            else if (x == null)
                return -1;
            else if (y == null)
                return 1;

            var byMatchesPlayed = CompareByMatchesPlayed(x, y);
            if (byMatchesPlayed != 0)
            {
                return byMatchesPlayed;
            }

            return CompareByPoints(x, y);
        }

        private int CompareByPoints(RankingItem<TeamDetails> x, RankingItem<TeamDetails> y)
        {
            return x.Points - y.Points;
        }

        private int CompareByMatchesPlayed(RankingItem<TeamDetails> x, RankingItem<TeamDetails> y)
        {
            return x.Details.MatchesPlayed - y.Details.MatchesPlayed;
        }

        
    }
}
