using ImbaBetWeb.Business.Ranking.Details;

namespace ImbaBetWeb.Business.Ranking.Comparer
{
    public class PlayerComparer : IComparer<RankingItem<PlayerDetails>>
    {
        public int Compare(RankingItem<PlayerDetails>? x, RankingItem<PlayerDetails>? y)
        {
            if (x == null && y == null)
                return 0;
            else if (x == null)
                return -1;
            else if (y == null)
                return 1;
            else
                return x.Points - y.Points; // Descending order
        }
    }
}
