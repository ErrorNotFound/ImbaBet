using ImbaBetWeb.Logic.Ranking.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImbaBetWeb.Logic.Ranking.Comparing
{
    public class TeamRankingComparer : IComparer<RankingItem<TeamDetails>>
    {
        public int Compare(RankingItem<TeamDetails>? x, RankingItem<TeamDetails>? y)
        {
            if (x == null && y == null)
                return 0;
            else if (x == null)
                return 1;
            else if (y == null)
                return -1;

            var byMatchesPlayed = CompareByMatchesPlayedDescending(x, y);
            if (byMatchesPlayed != 0)
            {
                return byMatchesPlayed;
            }

            return CompareByPointsDescending(x, y);
        }

        private int CompareByPointsDescending(RankingItem<TeamDetails> x, RankingItem<TeamDetails> y)
        {
            return y.Points - x.Points;
        }

        private int CompareByMatchesPlayedDescending(RankingItem<TeamDetails> x, RankingItem<TeamDetails> y)
        {
            return y.Details.MatchesPlayed - x.Details.MatchesPlayed;
        }

        
    }
}
