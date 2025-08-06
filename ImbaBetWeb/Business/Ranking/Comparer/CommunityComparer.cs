using ImbaBetWeb.Business.Ranking.Details;

namespace ImbaBetWeb.Business.Ranking.Comparer
{
    public class CommunityComparer : IComparer<RankingItem<CommunityDetails>>
    {
        public int Compare(RankingItem<CommunityDetails>? x, RankingItem<CommunityDetails>? y)
        {
            if (x == null && y == null)
                return 0;
            else if (x == null)
                return -1;
            else if (y == null)
                return 1;

            var byAveragePoints = CompareByAveragePoints(x, y);
            if (byAveragePoints != 0)
            {
                return byAveragePoints;
            }

            var byTotalPoints = CompareByTotalPoints(x, y);
            if (byTotalPoints != 0)
            {
                return byTotalPoints;
            }

            return CompareByMemberCount(x, y);
        }
        private int CompareByAveragePoints(RankingItem<CommunityDetails> x, RankingItem<CommunityDetails> y)
        {
            if (y.Details.AveragePoints == x.Details.AveragePoints)
                return 0;

            return x.Details.AveragePoints > y.Details.AveragePoints ? 1 : -1;
        }

        private int CompareByTotalPoints(RankingItem<CommunityDetails> x, RankingItem<CommunityDetails> y)
        {
            return x.Points - y.Points;
        }

        private int CompareByMemberCount(RankingItem<CommunityDetails> x, RankingItem<CommunityDetails> y)
        {
            return x.Details.MemberCount - y.Details.MemberCount;
        }
    }
}
