using ImbaBetWeb.Logic.Ranking.Details;

namespace ImbaBetWeb.Logic.Ranking.Comparing
{
    public class UserComparer : IComparer<RankingItem<UserDetails>>
    {
        public int Compare(RankingItem<UserDetails>? x, RankingItem<UserDetails>? y)
        {
            if (x == null && y == null)
                return 0;
            else if (x == null)
                return 1;
            else if (y == null)
                return -1;
            else
                return y.Points - x.Points; // Descending order
        }
    }
}
