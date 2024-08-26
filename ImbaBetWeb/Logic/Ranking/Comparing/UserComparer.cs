using ImbaBetWeb.Logic.Ranking.Details;

namespace ImbaBetWeb.Logic.Ranking.Comparing
{
    public class UserComparer : IComparer<RankingItem<UserDetails>>
    {
        public int Compare(RankingItem<UserDetails>? x, RankingItem<UserDetails>? y)
        {
            // Descending order
            return y.Points - x.Points;
        }
    }
}
