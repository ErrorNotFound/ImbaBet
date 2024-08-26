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
            var byMatchesPlayed = CompareByMatchesPlayedDescending(x, y);
            if (byMatchesPlayed != 0)
            {
                return byMatchesPlayed;
            }

            return CompareByPointsDescending(x, y);
        }

        private int CompareByPointsDescending(RankingItem<TeamDetails>? x, RankingItem<TeamDetails>? y)
        {

            return y.Points - x.Points;
        }

        private int CompareByMatchesPlayedDescending(RankingItem<TeamDetails>? x, RankingItem<TeamDetails>? y)
        {
            return y.Details.MatchesPlayed - x.Details.MatchesPlayed;
        }

        
    }
}
