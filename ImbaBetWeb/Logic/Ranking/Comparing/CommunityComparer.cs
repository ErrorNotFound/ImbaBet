using ImbaBetWeb.Logic.Ranking.Details;

namespace ImbaBetWeb.Logic.Ranking.Comparing
{
    public class CommunityComparer : IComparer<RankingItem<CommunityDetails>>
    {
        public int Compare(RankingItem<CommunityDetails>? x, RankingItem<CommunityDetails>? y)
        {
            var byAveragePoints = CompareByAveragePointsDescending(x, y);
            if (byAveragePoints != 0)
            {
                return byAveragePoints;
            }

            var byTotalPoints = CompareByTotalPointsDescending(x, y);
            if (byTotalPoints != 0)
            {
                return byTotalPoints;
            }

            return CompareByMemberCountDescending(x, y);
        }
        private int CompareByAveragePointsDescending(RankingItem<CommunityDetails>? x, RankingItem<CommunityDetails>? y)
        {
            if (y.Details.AveragePoints == x.Details.AveragePoints)
                return 0;

            return y.Details.AveragePoints > x.Details.AveragePoints ? 1 : -1;
        }

        private int CompareByTotalPointsDescending(RankingItem<CommunityDetails>? x, RankingItem<CommunityDetails>? y)
        {
            return y.Points - x.Points;
        }

        private int CompareByMemberCountDescending(RankingItem<CommunityDetails>? x, RankingItem<CommunityDetails>? y)
        {
            return y.Details.MemberCount - x.Details.MemberCount;
        }
    }
}
