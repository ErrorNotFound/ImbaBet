using ImbaBetWeb.Business.Ranking.Details;
using ImbaBetWeb.Business.Ranking;
using ImbaBetWeb.Model;

namespace ImbaBetWeb.ViewModels.Betting
{
    public class LeaderboardsViewModel
    {
        public required IList<RankingItem<PlayerDetails>> PlayerRanking { get; set; }
        public required IList<RankingItem<CommunityDetails>> CommunityRanking { get; set; }

        public required IList<RankingItem<PlayerDetails>>? CommunityInternalRanking { get; set; }
    }
}
